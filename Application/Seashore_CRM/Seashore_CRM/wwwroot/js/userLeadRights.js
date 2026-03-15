document.addEventListener('DOMContentLoaded', function () {
    const rightsTableBody = document.querySelector('#rightsTable tbody');
    const rightModalEl = document.getElementById('rightModal');
    const rightModal = new bootstrap.Modal(rightModalEl);
    const form = document.getElementById('rightForm');
    const modalTitle = document.getElementById('modalTitle');

    async function loadList() {
        const resp = await fetch('/UserLeadRights/List');
        if (!resp.ok) return console.error('Failed to load rights');
        const data = await resp.json();
        renderRows(data);
    }

    function renderRows(items) {
        rightsTableBody.innerHTML = '';
        items.forEach(i => {
            const tr = document.createElement('tr');

            tr.innerHTML = `
                <td>${escapeHtml(i.userName || '')}</td>
                <td>${escapeHtml(i.leadName || '')}</td>
                <td class="text-center">${i.canView ? '??' : '?'}</td>
                <td class="text-center">${i.canEdit ? '??' : '?'}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-primary btn-edit" data-id="${i.id}">Edit</button>
                    <button class="btn btn-sm btn-danger btn-delete" data-id="${i.id}">Delete</button>
                </td>
            `;
            rightsTableBody.appendChild(tr);
        });

        // wire buttons
        document.querySelectorAll('.btn-edit').forEach(b => b.addEventListener('click', onEdit));
        document.querySelectorAll('.btn-delete').forEach(b => b.addEventListener('click', onDelete));
    }

    function escapeHtml(s) {
        if (!s) return '';
        return s.replace(/[&<>"']/g, function (m) {
            return ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[m];
        });
    }

    document.getElementById('btnAddRight').addEventListener('click', function () {
        form.reset();
        form.Id.value = 0;
        modalTitle.textContent = 'Add Right';
        rightModal.show();
    });

    async function onEdit(e) {
        const id = e.currentTarget.dataset.id;
        const resp = await fetch(`/UserLeadRights/Get/${id}`);
        if (!resp.ok) return alert('Failed to load');
        const vm = await resp.json();

        form.Id.value = vm.id || 0;
        form.UserId.value = vm.userId || '';
        form.LeadId.value = vm.leadId || '';
        form.CanView.checked = vm.canView || false;
        form.CanEdit.checked = vm.canEdit || false;

        modalTitle.textContent = 'Edit Right';
        rightModal.show();
    }

    async function onDelete(e) {
        if (!confirm('Delete this right?')) return;
        const id = e.currentTarget.dataset.id;
        // anti-forgery token
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const body = new URLSearchParams();
        if (token) body.append('__RequestVerificationToken', token);

        const resp = await fetch(`/UserLeadRights/Delete/${id}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: body.toString()
        });

        if (!resp.ok) return alert('Delete failed');
        const j = await resp.json();
        if (j.success) loadList();
        else alert('Delete failed');
    }

    form.addEventListener('submit', async function (ev) {
        ev.preventDefault();

        // basic validation
        if (!form.UserId.value) return alert('Select user');
        if (!form.LeadId.value) return alert('Select lead');

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const data = new URLSearchParams();
        if (token) data.append('__RequestVerificationToken', token);

        data.append('Id', form.Id.value || '0');
        data.append('UserId', form.UserId.value);
        data.append('LeadId', form.LeadId.value);
        if (form.CanView.checked) data.append('CanView', 'true');
        if (form.CanEdit.checked) data.append('CanEdit', 'true');

        const resp = await fetch('/UserLeadRights/Save', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: data.toString()
        });

        if (!resp.ok) {
            const txt = await resp.text();
            alert('Save failed: ' + txt);
            return;
        }

        const j = await resp.json();
        if (j.success) {
            rightModal.hide();
            loadList();
        } else {
            alert('Save failed');
        }
    });

    // initial load
    loadList();
});

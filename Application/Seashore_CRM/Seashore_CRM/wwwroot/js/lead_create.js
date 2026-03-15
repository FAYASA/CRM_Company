$(document).ready(function () {
    let idx = 1; // For dynamic product row indexes

    // Global mappings
    const productsMap = window.productsMap || {}; // { "ProductName": { unitPrice, cost, tax, categoryId } }
    const statusMapping = window.statusMapping || {}; // { "StatusName": ["Activity1", "Activity2"] }
    const urls = window.urls || {}; // AJAX URLs

    /********** Suggested Activities **********/
    function renderSuggestedActivities() {
        const area = $('#suggestedActivitiesArea');
        area.empty();

        const statusText = $('#statusSelect option:selected').text();
        if (!statusText || statusText === '-- Select Status --') {
            area.append('<p class="text-muted small mb-0">Select a lead status to view suggestions.</p>');
            return;
        }

        const acts = statusMapping[statusText];
        if (acts && acts.length) {
            acts.forEach(a => {
                const idSafe = 'act_' + a.replace(/[^a-z0-9]/gi, '_');
                area.append(`
                    <div class="form-check form-check-inline">
                        <input type="checkbox" class="btn-check activity-chk" id="${idSafe}" value="${a}">
                        <label class="btn btn-outline-primary btn-sm rounded-pill px-3 mb-2" for="${idSafe}">${a}</label>
                    </div>
                `);
            });
        } else {
            area.append('<p class="text-muted small">No suggestions.</p>');
        }
    }

    renderSuggestedActivities();

    /********** Lead Type Toggle **********/
    function toggleCompany() {
        const isCorporate = $('#leadCorporate').is(':checked');

        $('#companySelect').prop('disabled', !isCorporate);

        if (!isCorporate) {
            $('#corporateContactDiv, #companySelectDiv').hide();
            $('#companySelect').val("");

            $('#individualContactDiv').show();
            $('#individualContactSelect').prop('disabled', false);

            $('#corporateContactSelect').empty().append('<option value="">-- Select Contact --</option>').prop('disabled', true);
        } else {
            $('#corporateContactDiv, #companySelectDiv').show();
            $('#individualContactDiv').hide();
            $('#individualContactSelect').prop('disabled', true);

            if ($('#companySelect').val()) $('#companySelect').trigger('change');
            else $('#corporateContactSelect').empty().append('<option value="">-- Select Contact --</option>').prop('disabled', true);
        }
    }

    $('input[name="LeadType"]').on('change', toggleCompany);
    toggleCompany();

    /********** Priority Buttons **********/
    $(document).on('click', '.priority-btn', function () {
        $('.priority-btn').removeClass('active btn-danger btn-warning btn-success').addClass('btn-outline-secondary');

        const val = $(this).data('val');
        $(this).addClass('active');

        if (val === 'Hot') $(this).removeClass('btn-outline-secondary').addClass('btn-danger');
        if (val === 'Warm') $(this).removeClass('btn-outline-secondary').addClass('btn-warning');
        if (val === 'Cold') $(this).removeClass('btn-outline-secondary').addClass('btn-success');

        $('#priorityHidden').val(val);
    });

    /********** Product Rows **********/
    $('#addProduct').on('click', function () {
        const row = $('.product-row').first().clone();

        // Reset values
        row.find('input').each(function () {
            if ($(this).hasClass('qty')) $(this).val(1);
            else if ($(this).hasClass('unitprice') || $(this).hasClass('cost') || $(this).hasClass('taxpct')) $(this).val(0);
            else $(this).val('');
        });
        row.find('select').val("");

        // Update name indexes
        row.find('[name]').each(function () {
            const name = $(this).attr('name');
            if (name) $(this).attr('name', name.replace(/\[\d+\]/, `[${idx}]`));
        });

        $('#productBody').append(row);
        idx++;
    });

    $(document).on('click', '.remove-row', function () {
        if ($('.product-row').length > 1) $(this).closest('tr').remove();
    });

    /********** Calculate Totals **********/
    function recalc(tr) {
        const q = parseFloat(tr.find('.qty').val() || 0);
        const u = parseFloat(tr.find('.unitprice').val() || 0);
        const tP = parseFloat(tr.find('.taxpct').val() || 0);
        const c = parseFloat(tr.find('.cost').val() || 0);

        const s = q * u;
        const tV = s * (tP / 100);
        const g = s + tV;
        const p = s - (c * q);

        tr.find('.salevalue').val(isNaN(s) ? '' : s.toFixed(2));
        tr.find('.taxvalue').val(isNaN(tV) ? '' : tV.toFixed(2));
        tr.find('.gtotal').val(isNaN(g) ? '' : g.toFixed(2));
        tr.find('.grossprofit').val(isNaN(p) ? '' : p.toFixed(2));
    }

    $(document).on('change', '.product-select', function () {
        const tr = $(this).closest('tr');
        const productName = $(this).val();
        const p = productsMap[productName];
        if (p) {
            tr.find('.unitprice').val(p.unitPrice ?? 0);
            tr.find('.cost').val(p.cost ?? 0);
            tr.find('.taxpct').val(p.tax ?? 0);
            tr.find('.category-select').val(p.categoryId || '');
        }
        recalc(tr);
    });

    $(document).on('input change', '.qty, .unitprice, .taxpct, .cost', function () {
        recalc($(this).closest('tr'));
    });

    /********** Company Contacts AJAX **********/
    $('#companySelect').on('change', function () {
        const companyId = $(this).val();
        const $contactSelect = $('#corporateContactSelect');

        if (!companyId) {
            $contactSelect.empty().append('<option value="">-- Select Contact --</option>').prop('disabled', true);
            return;
        }

        $.ajax({
            url: urls.contactsByCompany,
            data: { companyId: companyId },
            success: function (data) {
                $contactSelect.empty().append('<option value="">-- Select Contact --</option>');
                if (data && data.length) {
                    data.forEach(c => $contactSelect.append(`<option value="${c.id}">${c.name}</option>`));
                    $contactSelect.prop('disabled', false);
                } else {
                    $contactSelect.prop('disabled', true);
                }
            }
        });
    });

    /********** Status Activities AJAX **********/
    $('#statusSelect').on('change', function () {
        const statusId = $(this).val();
        const $activitySelect = $('#StatusActivitySelect');

        renderSuggestedActivities();

        if (!statusId) {
            $activitySelect.empty().append('<option value="">-- Select Activities --</option>').prop('disabled', true);
            return;
        }

        $.ajax({
            url: urls.activitiesByStatus,
            data: { statusId: statusId },
            success: function (data) {
                $activitySelect.empty().append('<option value="">-- Select Activities --</option>');
                if (data && data.length > 0) {
                    data.forEach(a => $activitySelect.append(`<option value="${a.id}">${a.name}</option>`));
                    $activitySelect.prop('disabled', false);
                } else {
                    $activitySelect.prop('disabled', true);
                }
            }
        });
    });

    /********** Modals: Save Contact **********/
    $('#saveContactBtn').click(function () {
        const name = $('#contactName').val();
        const phone = $('#contactPhone').val();
        const email = $('#contactEmail').val();

        if (!name) return alert("Please enter contact name");

        const newOption = `<option value="${name}" selected>${name}</option>`;
        $('#individualContactSelect').append(newOption);

        const modal = bootstrap.Modal.getInstance(document.getElementById('createContactModal'));
        modal.hide();

        // Clear modal inputs
        $('#contactName,#contactPhone,#contactEmail,#contactLocation').val('');
    });

    /********** Modals: Save Product **********/
    $('#saveNewProduct').click(function () {
        const name = $('#ProductName').val();
        if (!name) return alert("Enter product name");

        const newOption = `<option value="${name}" selected>${name}</option>`;
        $('input[list="productList"]').last().val(name);

        const modal = bootstrap.Modal.getInstance(document.getElementById('addProductModal'));
        modal.hide();

        // Clear modal inputs
        $('#ProductName,#newProductCost,#newProductTax').val('');
    });

    /********** Modals: Save Activity **********/
    $('#saveActivityBtn').click(function () {
        const actName = $('#newActivityName').val();
        if (!actName) return alert("Enter activity name");

        const status = $('#modalStatusName').val();
        if (!statusMapping[status]) statusMapping[status] = [];
        statusMapping[status].push(actName);

        renderSuggestedActivities();

        const modal = bootstrap.Modal.getInstance(document.getElementById('addActivityModal'));
        modal.hide();
        $('#newActivityName').val('');
    });

});
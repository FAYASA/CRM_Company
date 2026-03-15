(function ($) {
    $(function () {
        // =======================
        // 1. Initialize Variables
        // =======================
        let idx = 1;
        const productsMap = (window.leadsCreateConfig && window.leadsCreateConfig.productsMap) || {};
        const initialItems = (window.leadsCreateConfig && window.leadsCreateConfig.initialItems) || [];
        const initialPriority = (window.leadsCreateConfig && window.leadsCreateConfig.initialPriority) || null;
        const initialActivityType = (window.leadsCreateConfig && window.leadsCreateConfig.initialActivityType) || null;
        const initialSelectedActivities = (window.leadsCreateConfig && window.leadsCreateConfig.initialSelectedActivities) || [];
        let currentProductSelect = null;
        // rights index starts after existing rows
        let rightsIndex = ($('#rightsTableBody tr').length) || 0;

        // =======================
        // Utility: recalc row totals
        // =======================
        function recalcRow(tr) {
            const q = parseFloat(tr.find('.qty').val() || 0);
            const u = parseFloat(tr.find('.unitprice').val() || 0);
            const tP = parseFloat(tr.find('.taxpct').val() || 0);
            const c = parseFloat(tr.find('.cost').val() || 0);

            const s = q * u;
            const tV = s * (tP / 100);
            const g = s + tV;
            const p = s - (c * q);

            tr.find('.salevalue').val(s.toFixed(2));
            tr.find('.taxvalue').val(tV.toFixed(2));
            tr.find('.gtotal').val(g.toFixed(2));
            tr.find('.grossprofit').val(p.toFixed(2));

            // also update hidden inputs on the row if present (for server binding)
            const indexMatch = tr.find('.product-id').attr('name') ? tr.find('.product-id').attr('name').match(/Lead\.ProductItems\[(\d+)\]/) : null;
            if (indexMatch) {
                const i = indexMatch[1];
                // ensure hidden inputs for calculated values exist and are updated
                updateOrCreateHidden(tr, `Lead.ProductItems[${i}].SaleValue`, s.toFixed(2));
                updateOrCreateHidden(tr, `Lead.ProductItems[${i}].TaxValue`, tV.toFixed(2));
                updateOrCreateHidden(tr, `Lead.ProductItems[${i}].GrossTotal`, g.toFixed(2));
                updateOrCreateHidden(tr, `Lead.ProductItems[${i}].GrossProfit`, p.toFixed(2));
            }
        }

        function updateOrCreateHidden(tr, name, value) {
            let hd = tr.find(`input[type=hidden][name="${name}"]`);
            if (hd.length) {
                hd.val(value);
            } else {
                tr.append(`<input type="hidden" name="${name}" value="${value}" />`);
            }
        }

        // =======================
        // Aggregate totals
        // =======================
        function recalcAll() {
            let subTotal = 0;
            let taxTotal = 0;
            let grossTotal = 0;
            let totalCost = 0;
            let grossProfit = 0;

            $('#productBody tr').each(function () {
                const tr = $(this);
                const s = parseFloat(tr.find('.salevalue').val() || 0);
                const tV = parseFloat(tr.find('.taxvalue').val() || 0);
                const g = parseFloat(tr.find('.gtotal').val() || 0);
                const c = parseFloat(tr.find('.cost').val() || 0);
                const q = parseFloat(tr.find('.qty').val() || 0);
                const p = parseFloat(tr.find('.grossprofit').val() || 0);

                subTotal += s;
                taxTotal += tV;
                grossTotal += g;
                totalCost += (c * q);
                grossProfit += p;
            });

            // update UI fields if present
            $('#leadSubtotal').text(subTotal.toFixed(2));
            $('#leadTaxTotal').text(taxTotal.toFixed(2));
            $('#leadGrossTotal').text(grossTotal.toFixed(2));
            $('#leadTotalCost').text(totalCost.toFixed(2));
            $('#leadGrossProfit').text(grossProfit.toFixed(2));

            // update hidden inputs for server
            setHidden('Lead.SubTotal', subTotal.toFixed(2));
            setHidden('Lead.TaxTotal', taxTotal.toFixed(2));
            setHidden('Lead.GrossTotal', grossTotal.toFixed(2));
            setHidden('Lead.TotalCost', totalCost.toFixed(2));
            setHidden('Lead.GrossProfit', grossProfit.toFixed(2));
        }

        function setHidden(name, value) {
            let el = $(`input[type=hidden][name="${name}"]`);
            if (el.length) el.val(value);
            else $('#productTable').after(`<input type="hidden" name="${name}" value="${value}" />`);
        }

        // =======================
        // Lead Type Logic
        // =======================
        function toggleCompany() {
            const isCorporate = $('#leadCorporate').is(':checked');
            $('#companySelect').prop('disabled', !isCorporate);

            if (!isCorporate) {
                $('#corporateContactDiv, #companySelectDiv').hide();
                $('#companySelect').val('');
                $('#individualContactDiv').show();
                $('#individualContactSelect').prop('disabled', false).val('');
                $('#corporateContactSelect').empty().append('<option value="">-- Select Contact --</option>').prop('disabled', true);
            } else {
                $('#corporateContactDiv, #companySelectDiv').show();
                $('#individualContactDiv').hide();
                $('#individualContactSelect').prop('disabled', true);
                if ($('#companySelect').val()) $('#companySelect').trigger('change');
                else $('#corporateContactSelect').empty().append('<option value="">-- Select Contact --</option>').prop('disabled', true);
            }
        }
        $('input[name="Lead.LeadType"]').on('change', toggleCompany);
        toggleCompany();

        // =======================
        // Priority Button Group
        // =======================
        function setPriorityButtons(val) {
            $('.priority-btn').removeClass('active btn-danger btn-warning btn-success').addClass('btn-outline-secondary');
            $('.priority-btn').each(function () {
                if ($(this).data('val') === val) {
                    $(this).addClass('active');
                    if (val === 'Hot') $(this).removeClass('btn-outline-secondary').addClass('btn-danger');
                    if (val === 'Warm') $(this).removeClass('btn-outline-secondary').addClass('btn-warning');
                    if (val === 'Cold') $(this).removeClass('btn-outline-secondary').addClass('btn-success');
                }
            });
            $('#priorityHidden').val(val || '');
        }

        $('.priority-btn').click(function () {
            const val = $(this).data('val');
            setPriorityButtons(val);
        });

        // set initial priority from server if provided
        if (initialPriority) setPriorityButtons(initialPriority);

        // =======================
        // Activity Type buttons
        // =======================
        function setActivityType(val) {
            if (!val) return;
            $('input[name="Lead.ActivityType"]').prop('checked', false);
            $(`input[name="Lead.ActivityType"][value="${val}"]`).prop('checked', true);
        }
        if (initialActivityType) setActivityType(initialActivityType);

        // =======================
        // Status and Activity Change
        // =======================
        $('#statusSelect').on('change', function () {
            const statusId = $(this).val();
            const $activitySelect = $('#StatusActivitySelect');

            if (!statusId) {
                $activitySelect.empty().append('<option value="">-- Select Activities --</option>').prop('disabled', true);
                return;
            }

            $.get(window.leadsCreateConfig.urls.activitiesByStatus, { statusId }, function (data) {
                $activitySelect.empty().append('<option value="">-- Select Activities --</option>');
                if (data && data.length) {
                    data.forEach(a => $activitySelect.append('<option value="' + a.id + '">' + a.name + '</option>'));
                    $activitySelect.prop('disabled', false);

                    // if editing, select existing activity id by matching name
                    if (initialSelectedActivities && initialSelectedActivities.length) {
                        // try to select the first matching available activity
                        const firstMatch = data.find(d => initialSelectedActivities.includes(d.name));
                        if (firstMatch) $activitySelect.val(firstMatch.id);
                    }
                } else $activitySelect.prop('disabled', true);
            }).fail(() => console.log("Activity load failed"));
        });

        $('#addStatusActivityBtn').on('click', function (e) {
            e.preventDefault();
            const statusText = $('#statusSelect option:selected').text();
            $('#modalStatusName').val(statusText);
            new bootstrap.Modal(document.getElementById('addActivityModal')).show();
        });

        $('#saveActivityBtn').on('click', function () {
            const statusName = $('#modalStatusName').val();
            const activityName = $('#newActivityName').val().trim();
            if (!statusName || statusName === '-- Select Status --') return alert('Please select a status first.');
            if (!activityName) return alert('Enter activity name.');
            const token = $('input[name="__RequestVerificationToken"]').first().val();

            $.post(window.leadsCreateConfig.urls.addStatusActivity, {
                statusName, activityName, __RequestVerificationToken: token
            }, function (resp) {
                if (resp && resp.success) {
                    bootstrap.Modal.getInstance(document.getElementById('addActivityModal')).hide();
                    $('#newActivityName').val('');
                } else alert('Failed to add activity');
            }).fail(() => alert('Error while adding activity'));
        });

        // =======================
        // Comments Template
        // =======================
        $('#commentTemplate').on('change', function () {
            const txt = $(this).find("option:selected").text();
            if ($(this).val()) {
                const box = $('#comments');
                box.val((box.val() ? box.val() + "\n" : "") + txt);
                $(this).val("");
            }
        });

        // =======================
        // Product Row Management & Calculations
        // =======================
        function prepareRow(row, index) {
            // update name attributes for model binding
            row.find('input, select').each(function () {
                const cls = $(this).attr('class') || '';
                // map based on class to expected DTO property name
                if ($(this).hasClass('product-id')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].ProductId`);
                } else if ($(this).hasClass('product-name')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].ProductName`);
                } else if ($(this).hasClass('category-id')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].CategoryId`);
                } else if ($(this).hasClass('category')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].CategoryName`);
                } else if ($(this).hasClass('productgroup')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].ProductGroup`);
                } else if ($(this).hasClass('qty')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].Quantity`);
                } else if ($(this).hasClass('unitprice')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].UnitPrice`);
                } else if ($(this).hasClass('taxpct')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].TaxPercentage`);
                } else if ($(this).hasClass('cost')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].Cost`);
                } else if ($(this).hasClass('salevalue')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].SaleValue`);
                } else if ($(this).hasClass('taxvalue')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].TaxValue`);
                } else if ($(this).hasClass('gtotal')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].GrossTotal`);
                } else if ($(this).hasClass('grossprofit')) {
                    $(this).attr('name', `Lead.ProductItems[${index}].GrossProfit`);
                }
            });

            // bind change events for calculation
            row.find('.qty, .unitprice, .taxpct, .cost').off('input change').on('input change', function () {
                recalcRow(row);
                recalcAll();
            });

            // ensure remove button exists and bound
            row.find('.remove-row').off('click').on('click', function () {
                if ($('#productBody tr').length > 1) {
                    row.remove();
                    recalcAll();
                } else {
                    alert('At least one product row is required.');
                }
            });
        }

        $('#addProduct').click(function () {
            const index = $('#productBody tr').length;
            const newRow = $('#productBody tr:first').clone();
            // remove values in clone
            newRow.find('input, select').val('');
            prepareRow(newRow, index);
            // remove any hidden calculated inputs to avoid duplicates
            newRow.find('input[type=hidden]').not('.product-id').remove();
            $('#productBody').append(newRow);
            recalcAll();
        });

        // initial preparation for existing first row
        prepareRow($('#productBody tr:first'), 0);

        $(document).on('click', '.remove-row', function () {
            if ($('#productBody tr').length > 1) $(this).closest('tr').remove();
            else alert('At least one product row is required.');
            recalcAll();
        });

        // If initialItems exist, render them into rows
        function renderInitialItems(items) {
            if (!items || !items.length) return;
            // clear existing rows
            $('#productBody').empty();
            items.forEach(function (it, i) {
                const newRow = $('#productBody tr.product-row').first().clone();
                // set display values (support camelCase or PascalCase)
                const productName = it.ProductName || it.productName || '';
                const productId = it.ProductId || it.productId || '';
                const quantity = it.Quantity || it.quantity || 1;
                const unitPrice = it.UnitPrice || it.unitPrice || 0;
                const taxPct = it.TaxPercentage || it.taxPercentage || 0;
                const cost = it.Cost || it.cost || 0;
                const categoryName = it.CategoryName || it.categoryName || '';
                const categoryId = it.CategoryId || it.categoryId || '';
                const productGroup = it.ProductGroup || it.productGroup || '';

                newRow.find('.product-name').val(productName);
                newRow.find('.product-id').val(productId);
                newRow.find('.qty').val(quantity);
                newRow.find('.unitprice').val(unitPrice);
                newRow.find('.taxpct').val(taxPct);
                newRow.find('.cost').val(cost);
                newRow.find('.category').val(categoryName);
                newRow.find('.category-id').val(categoryId);
                newRow.find('.productgroup').val(productGroup);

                // set proper name attributes
                prepareRow(newRow, i);

                // ensure hidden fields for server values are present (they will be created by prepareRow)
                recalcRow(newRow);
                $('#productBody').append(newRow);
            });
            recalcAll();
        }

        renderInitialItems(initialItems);

        // =======================
        // Rights: add / remove
        // =======================
        $('#addUserRight').on('click', function () {
            const userId = $('#userSelect').val();
            const userName = $('#userSelect option:selected').text().trim();
            const canView = $('#canView').is(':checked');
            const canEdit = $('#canEdit').is(':checked');

            if (!userId) return alert('Select a user');

            let existingInput = null;
            $('#rightsTableBody').find('input[type="hidden"]').each(function () {
                const name = $(this).attr('name') || '';
                if (name.endsWith('.UserId') && $(this).val() == userId) {
                    existingInput = $(this);
                    return false; // break
                }
            });

            function renderIcon(val) { return val ? '??' : '?'; }

            if (existingInput) {
                const name = existingInput.attr('name');
                const idx = name.substring(name.indexOf('[') + 1, name.indexOf(']'));
                const row = $('#rightsTableBody').find('tr[data-index="' + idx + '"]');

                row.find('td').eq(1).html(renderIcon(canView));
                row.find('td').eq(2).html(renderIcon(canEdit));

                let canViewInput = row.find('input[name="UserLeadRights[' + idx + '].CanView"]');
                if (canViewInput.length) canViewInput.val(canView);
                else row.find('td').eq(1).append('<input type="hidden" name="UserLeadRights[' + idx + '].CanView" value="' + canView + '" />');

                let canEditInput = row.find('input[name="UserLeadRights[' + idx + '].CanEdit"]');
                if (canEditInput.length) canEditInput.val(canEdit);
                else row.find('td').eq(2).append('<input type="hidden" name="UserLeadRights[' + idx + '].CanEdit" value="' + canEdit + '" />');

                alert('This user already exists. Rights have been updated.');

                $('#userSelect').val('');
                $('#canView, #canEdit').prop('checked', false);
                return;
            }

            const tr = $(`<tr data-index="${rightsIndex}"></tr>`);
            const tdUser = $(`<td></td>`).text(userName)
                .append(`<input type="hidden" name="UserLeadRights[${rightsIndex}].Id" value="0" />`)
                .append(`<input type="hidden" name="UserLeadRights[${rightsIndex}].UserId" value="${userId}" />`)
                .append(`<input type="hidden" name="UserLeadRights[${rightsIndex}].LeadId" value="" />`);
            const tdView = $(`<td></td>`).html(renderIcon(canView))
                .append(`<input type="hidden" name="UserLeadRights[${rightsIndex}].CanView" value="${canView}" />`);
            const tdEdit = $(`<td></td>`).html(renderIcon(canEdit))
                .append(`<input type="hidden" name="UserLeadRights[${rightsIndex}].CanEdit" value="${canEdit}" />`);
            const tdActions = $(`<td></td>`).html(`<button type="button" class="btn btn-sm btn-danger remove-right">Remove</button>`);

            tr.append(tdUser, tdView, tdEdit, tdActions);
            $('#rightsTableBody').append(tr);
            rightsIndex++;

            $('#userSelect').val('');
            $('#canView, #canEdit').prop('checked', false);
        });

        $(document).on('click', '.remove-right', function () {
            $(this).closest('tr').remove();
        });

        // =======================
        // AJAX: Contacts, Activities, Product Groups
        // =======================
        $('#companySelect').on('change', function () {
            const companyId = $(this).val();
            const $contactSelect = $('#corporateContactSelect');

            if (!companyId) {
                $contactSelect.empty().append('<option value="">-- Select Contact --</option>').prop('disabled', true);
                return;
            }

            $.get(window.leadsCreateConfig.urls.contactsByCompany, { companyId }, function (data) {
                $contactSelect.empty().append('<option value="">-- Select Contact --</option>');
                if (data && data.length) {
                    data.forEach(c => $contactSelect.append('<option value="' + c.id + '">' + c.name + '</option>'));
                    $contactSelect.prop('disabled', false);
                } else $contactSelect.prop('disabled', true);
            }).fail(() => $contactSelect.prop('disabled', true));
        });

        $('#newProductCategory').on('change', function () {
            const categoryId = $(this).val();
            const $proGroupSelect = $('#newPro_Group');

            if (!categoryId) {
                $proGroupSelect.empty().append('<option value="">-- Select Pro.Group --</option>').prop('disabled', true);
                return;
            }

            $.get(window.leadsCreateConfig.urls.productGroupsByCategory, { categoryId }, function (data) {
                $proGroupSelect.empty().append('<option value="">-- Select Pro.Group --</option>');
                if (data && data.length) {
                    data.forEach(g => $proGroupSelect.append('<option value="' + g.id + '">' + g.name + '</option>'));
                    $proGroupSelect.prop('disabled', false);
                } else $proGroupSelect.prop('disabled', true);
            }).fail(() => $proGroupSelect.prop('disabled', true));
        });

        // =======================
        // Modal AJAX: Products
        // =======================
        $(document).on("click", ".add-product-btn", function () {
            currentProductSelect = $(this).closest("td").find(".product-select");
            new bootstrap.Modal(document.getElementById('addProductModal')).show();
        });

        $("#saveNewProduct").click(function () {
            const data = {
                ProductName: $("#newProductName").val(),
                CategoryId: $("#newProductCategory").val(),
                ProductGroupId: $("#newPro_Group").val(),
                Cost: $("#newProductCost").val(),
                TaxPercentage: $("#newProductTax").val()
            };

            if (!data.ProductName) return alert("Enter product name");
            if (!data.CategoryId) return alert("Select category");
            if (!data.Cost) return alert("Enter cost");

            $.post(window.leadsCreateConfig.urls.quickCreateProduct, data, function (res) {

                if (res.success) {

                    // Use categoryName and productGroupName (not IDs) for data attributes so UI shows names immediately
                    $("#productList").append(`\n                        <option value="${res.name}"\n                                data-id="${res.id}"\n                                data-category="${res.categoryName || ''}"\n                                data-progroup="${res.productGroupName || ''}">\n                        </option>\n                    `);

                    // update local product map with human-readable names
                    productsMap[res.id] = {
                        cost: res.cost,
                        tax: res.tax,
                        categoryName: res.categoryName || '',
                        productGroupName: res.productGroupName || ''
                    };

                    bootstrap.Modal.getInstance(document.getElementById('addProductModal')).hide();

                    $("#newProductName, #newProductCategory, #newPro_Group, #newProductCost, #newProductTax").val('');

                } else {
                    alert(res.message || "Error saving product");
                }

            }).fail(() => alert("AJAX error. Please try again."));
        });

        // =======================
        // Modal AJAX: Individual Contact
        // =======================
        $("#saveContactBtn").click(function () {
            const data = {
                Name: $("#contactName").val(),
                Phone: $("#contactPhone").val(),
                Email: $("#contactEmail").val(),
                Location: $("#contactLocation").val()
            };

            fetch(window.leadsCreateConfig.urls.createIndividualContact, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            })
                .then(r => r.json())
                .then(res => {
                    if (res.success) {
                        const select = $("#individualContactSelect");
                        select.append(`<option value="${res.id}" selected>${res.name}</option>`);
                        bootstrap.Modal.getInstance(document.getElementById('createContactModal')).hide();
                    }
                });
        });

        // =======================
        // Product Auto Fill on selection
        // =======================
        $(document).on('change', '.product-name', function () {
            const val = $(this).val();
            const row = $(this).closest('tr');

            if (!val) {
                row.find('.category, .productgroup, .cost, .taxpct, .product-id').val('');
                recalcRow(row);
                recalcAll();
                return;
            }

            const option = $('#productList option').filter(function () { return $(this).val() === val; }).first();
            let productId = null;

            if (option.length) {
                const category = option.data('category') || '';
                const proGroup = option.data('progroup') || '';
                productId = option.data('id') || '';

                row.find('.category').val(category);
                row.find('.productgroup').val(proGroup);
                row.find('.product-id').val(productId);
            } else {
                productId = val;
                row.find('.product-id').val(productId);
            }

            if (productId && productsMap && productsMap[productId]) {
                // prefer name fields from productsMap when available
                if (productsMap[productId].categoryName) row.find('.category').val(productsMap[productId].categoryName);
                if (productsMap[productId].productGroupName) row.find('.productgroup').val(productsMap[productId].productGroupName);
                if (productsMap[productId].cost !== undefined) row.find('.cost').val(productsMap[productId].cost);
                if (productsMap[productId].tax !== undefined) row.find('.taxpct').val(productsMap[productId].tax);
                recalcRow(row);
                recalcAll();
                return;
            }

            if (productId) {
                $.get(window.leadsCreateConfig.urls.getProductDetails, { id: productId }, function (res) {
                    if (!res) return;

                    if (res.cost !== undefined) row.find('.cost').val(res.cost);
                    if (res.tax !== undefined) row.find('.taxpct').val(res.tax);
                    if (res.categoryName) row.find('.category').val(res.categoryName);
                    if (res.productGroupName) row.find('.productgroup').val(res.productGroupName);

                    recalcRow(row);
                    recalcAll();
                }).fail(function () {
                    console.log('Failed to load product details for id:', productId);
                });
            } else {
                recalcRow(row);
                recalcAll();
            }
        });

        // Trigger change on pre-selected company/status
        if ($('#companySelect').val()) $('#companySelect').trigger('change');
        if ($('#statusSelect').val()) $('#statusSelect').trigger('change');

        // initial totals
        recalcAll();

    });
})(jQuery);

src = "~/Scripts/jquery-1.10.2.min.js";

$(document).ready(function () {
    const initialNum = 1;  // 初始化第一個計畫的編號
    updatePlanNames(initialNum);

    // 清空第一個計畫的所有欄位
    $(`#accountNo${initialNum}`).val('');
    $(`#planNo${initialNum}`).val('');
    $(`#CommissionName${initialNum}`).val('');
    $(`#planStartInput${initialNum}`).val('');
    $(`#planEndInput${initialNum}`).val('');

    // 預設加載經費來源對應的計畫名稱
    $(document).on('change', '[id^=fundingSource]', function () {
        const num = $(this).attr('id').replace('fundingSource', '');
        updatePlanNames(num);
    });

    // 計畫名稱改變時加載計畫詳細資料
    $(document).on('change', '[id^=planName]', function () {
        const num = $(this).attr('id').replace('planName', '');
        updatePlanDetails(num);
    });
});

function updatePlanNames(num) {
    const fundingSource = $(`#fundingSource${num}`).val();
    console.log('Selected funding source:', fundingSource);
    $.ajax({
        url: '/NewHire/GetPlansByFundingSource',
        type: 'GET',
        data: { fundingSource: fundingSource },
        dataType: 'json',
        success: function (data) {
            const planNameSelect = $(`#planName${num}`);
            planNameSelect.empty();
            planNameSelect.append($('<option></option>').val('').text('請選擇計畫名稱'));

            if (Array.isArray(data)) {
                $.each(data, function (i, item) {
                    planNameSelect.append($('<option></option>').val(item.value).text(item.text));
                });
            } else {
                planNameSelect.append($('<option></option>').val('').text('無法加載計畫名稱'));
            }
        },
        error: function (xhr, status, error) {
            console.error('Error fetching plan names:', error);
            const planNameSelect = $(`#planName${num}`);
            planNameSelect.empty();
            planNameSelect.append($('<option></option>').val('').text('請選擇計畫名稱'));
            planNameSelect.append($('<option></option>').val('').text('無法加載計畫名稱'));
        }
    });
}

function updatePlanDetails(num) {
    const planName = $(`#planName${num}`).val();
    console.log('Selected plan name:', planName);
    if (planName) {
        $.ajax({
            url: '/NewHire/GetPlanDetail',
            type: 'GET',
            data: { planName: planName },
            success: function (data) {
                $(`#accountNo${num}`).val(data.projectno.trim());
                $(`#planNo${num}`).val(data.projno.trim());
                $(`#CommissionName${num}`).val(data.comname.trim());
                $(`#planStartHidden${num}`).val(data.startdate.trim());
                $(`#planEndHidden${num}`).val(data.enddate.trim());

                if (data.startdate && data.startdate.trim().length === 7) {
                    const startYear = parseInt(data.startdate.trim().substr(0, 3)) + 1911;
                    const startMonth = parseInt(data.startdate.trim().substr(3, 2)) - 1;
                    const startDay = parseInt(data.startdate.trim().substr(5, 2));
                    const startDateObj = new Date(startYear, startMonth, startDay);
                    $(`#planStartInput${num}`).val(startDateObj.toISOString().split('T')[0]);
                }

                if (data.enddate && data.enddate.trim().length === 7) {
                    const endYear = parseInt(data.enddate.trim().substr(0, 3)) + 1911;
                    const endMonth = parseInt(data.enddate.trim().substr(3, 2)) - 1;
                    const endDay = parseInt(data.enddate.trim().substr(5, 2));
                    const endDateObj = new Date(endYear, endMonth, endDay);
                    $(`#planEndInput${num}`).val(endDateObj.toISOString().split('T')[0]);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error fetching plan details:', error);
            }
        });
    } else {
        $(`#accountNo${num}`).val('');
        $(`#planNo${num}`).val('');
        $(`#CommissionName${num}`).val('');
        $(`#planStartInput${num}`).val('');
        $(`#planEndInput${num}`).val('');
    }
}



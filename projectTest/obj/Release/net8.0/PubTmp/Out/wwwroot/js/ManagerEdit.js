document.querySelectorAll('input[name="salary"]').forEach(function (radio) {
    radio.addEventListener('change', function () {
        // 禁用所有薪水輸入框
        document.getElementById('monthlySalaryAmount').disabled = true;
        document.getElementById('dailySalaryAmount').disabled = true;
        document.getElementById('hourlySalaryAmount').disabled = true;

        // 啟用對應薪水輸入框
        if (this.checked) {
            if (this.value === '月薪') {
                document.getElementById('monthlySalaryAmount').disabled = false;
                document.getElementById('monthlySalaryAmount').focus();
            } else if (this.value === '日薪') {
                document.getElementById('dailySalaryAmount').disabled = false;
                document.getElementById('dailySalaryAmount').focus();
            } else if (this.value === '時薪') {
                document.getElementById('hourlySalaryAmount').disabled = false;
                document.getElementById('hourlySalaryAmount').focus();
            }
        }
    });
});

document.querySelectorAll('input[name="studentType"]').forEach(function (radio) {
    radio.addEventListener('change', function () {
        // 禁用他校學生輸入框
        document.getElementById('currentSchool').disabled = true;
        document.getElementById('department').disabled = true;

        // 啟用他校學生輸入框
        if (this.checked) {
            if (this.value === '他校學生') {
                document.getElementById('currentSchool').disabled = false;
                document.getElementById('department').disabled = false;
                document.getElementById('currentSchool').focus();
            }
        }
    });
});

document.querySelectorAll('input[name="commissionedUnit"]').forEach(function (radio) {
    radio.addEventListener('change', function () {
        // 啟用其他輸入框
        if (this.checked) {
            if (this.value === '其他') {
                document.getElementById('otherOrganisation').disabled = false;
                document.getElementById('otherOrganisation').focus();
            }
        }
    });
});

document.querySelectorAll('input[name="studentType"]').forEach(function (radio) {
    radio.addEventListener('change', function () {
        document.getElementById('hasOtherJob').disabled = true;
        document.getElementById('noOtherJob').disabled = true;

        if (this.value === '非在學學生') {
            document.getElementById('hasOtherJob').disabled = false;
            document.getElementById('noOtherJob').disabled = false;
        }
    });
});

document.getElementById('Requisition').addEventListener('submit', function (event) {
    var fundingSource = document.getElementById('fundingSource');
    var projectName = document.getElementById('projectName');
    var rankSelected = document.querySelector('input[name="rank"]:checked');
    var studentTypeSelected = document.querySelector('input[name="studentType"]:checked');
    var otherJobStatusSelected = document.querySelector('input[name="otherJobStatus"]:checked');
    var currentSchool = document.getElementById('currentSchool');
    var department = document.getElementById('department');
    var salarySelected = document.querySelector('input[name="salary"]:checked');
    var monthlySalaryAmount = document.getElementById('monthlySalaryAmount');
    var dailySalaryAmount = document.getElementById('dailySalaryAmount');
    var hourlySalaryAmount = document.getElementById('hourlySalaryAmount');
    var OrganisationSelected = document.querySelector('input[name="commissionedUnit"]:checked');
    var otherOrganisation = document.getElementById('otherOrganisation');

    if (fundingSource.value === 'none' || projectName.value === 'none' || !rankSelected || !studentTypeSelected || !salarySelected || !OrganisationSelected) {
        event.preventDefault();
        alert('欄位尚未填寫完成');
    }

    // 如果選擇其他委託機構但沒有填機構名稱
    if (OrganisationSelected && OrganisationSelected.value === '其他' && otherOrganisation.value === '') {
        event.preventDefault();
        alert('請填寫委託機構名稱');
    }

    // 如果選擇了非在學學生，必須選擇其他專職狀態
    if (studentTypeSelected && studentTypeSelected.value === '非在學學生' && !otherJobStatusSelected) {
        event.preventDefault();
        alert('請選擇是否有其他專職');
    }

    // 如果選擇了他校學生但沒有填寫就讀學校或系所，彈出警告
    if (studentTypeSelected && studentTypeSelected.value === '他校學生' && (currentSchool.value === '' || department.value === '')) {
        event.preventDefault();
        alert('請填寫就讀學校和系所');
    }

    // 如果選擇了薪水類型但沒有填寫薪水，彈出警告
    if (salarySelected) {
        if (salarySelected.value === '月薪' && monthlySalaryAmount.value === '') {
            event.preventDefault();
            alert('請填寫月薪金額');
        } else if (salarySelected.value === '日薪' && dailySalaryAmount.value === '') {
            event.preventDefault();
            alert('請填寫日薪金額');
        } else if (salarySelected.value === '時薪' && hourlySalaryAmount.value === '') {
            event.preventDefault();
            alert('請填寫時薪金額');
        }
    }
});

// 重置表單時恢復禁用
document.getElementById('Requisition').addEventListener('reset', function () {
    document.getElementById('monthlySalaryAmount').disabled = true;
    document.getElementById('dailySalaryAmount').disabled = true;
    document.getElementById('hourlySalaryAmount').disabled = true;
    document.getElementById('hasOtherJob').disabled = true;
    document.getElementById('noOtherJob').disabled = true;
    document.getElementById('otherOrganisation').disabled = true;
});


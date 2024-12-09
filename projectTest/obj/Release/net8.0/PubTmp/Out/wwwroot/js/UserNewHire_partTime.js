document.addEventListener('DOMContentLoaded', function () {
    // 重置表單時恢復禁用
    document.getElementById('Requisition').addEventListener('reset', function () {
        const idsToDisable = [
            'monthlySalaryAmount',
            'dailySalaryAmount',
            'hourlySalaryAmount',
            'hasOtherJob',
            'noOtherJob',
            'currentSchool',
            'department'
        ];
        idsToDisable.forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.disabled = true;
            }
        });
        document.getElementById('studyStatusMemo').value = ''; // 清空 studyStatusMemo
    });

    function updateStudentInfo() {
        var studentid = $('#studentID').val();
        console.log('StudentID:', studentid);
        if (studentid) {
            $.ajax({
                url: '/NewHire/GetStudentInfo',
                type: 'GET',
                data: { studentid: studentid },
                success: function (data) {
                    console.log('Received StudentInfo:', data);

                    $('#name').val(data.cname.trim());
                    $('#phone').val(data.phone.trim());
                    $('#ID').val(data.id.trim());
                    
                    $('#birthdayHidden').val(data.birthday.trim());
                    if (data.birthday && data.birthday.trim().length === 7) {
                        const year = parseInt(data.birthday.trim().substr(0, 3)) + 1911;
                        const month = parseInt(data.birthday.trim().substr(3, 2)) - 1;
                        const day = parseInt(data.birthday.trim().substr(5, 2)) + 1 ;
                        const dateObj = new Date(year, month, day);
                        
                        $('#birthdayInput').val(dateObj.toISOString().split('T')[0]);
                    }
                    
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching plan details:', error);
                    console.log('XHR status:', status);
                    console.log('XHR response:', xhr.responseText);
                }
            });
        }
    }

    $('#studentID').on('change', function () {
        $('#name').val('');
        $('#phone').val('');
        $('#ID').val('');

        updateStudentInfo();
    });

    // 初始化日期字段
    document.querySelectorAll('.date-input').forEach(input => {
        input.addEventListener('change', () => updateHiddenField(input));
        updateHiddenField(input); // 初始化时更新隐藏字段
    });

    function updateHiddenField(input) {
        const date = new Date(input.value);
        if (!isNaN(date.getTime())) {
            const rocYear = date.getFullYear() - 1911;
            const rocDate = `${rocYear.toString().padStart(3, '0')}${(date.getMonth() + 1).toString().padStart(2, '0')}${date.getDate().toString().padStart(2, '0')}`;
            document.getElementById(input.id.replace('Input', 'Hidden')).value = rocDate;
        }
    }

    function initializeDateField(fieldName, dateString) {
        if (dateString && dateString.length === 7) {
            const year = parseInt(dateString.substr(0, 3)) + 1911;
            const month = parseInt(dateString.substr(3, 2)) - 1;
            const day = parseInt(dateString.substr(5, 2)) + 1;
            const dateObj = new Date(year, month, day);
            document.getElementById(fieldName + 'Input').value = dateObj.toISOString().split('T')[0];
            document.getElementById(fieldName + 'Hidden').value = dateString;
        }
    }

    // 顯示返回表單後的日期
    ['birthday', 'employedStart', 'employedEnd', 'resignDate', 'SalaryChange'].forEach(field => {
        initializeDateField(field, document.getElementById(field + 'Hidden')?.value);
    });

    // 身份别初始化
    document.querySelectorAll('input[name="StudyStatus"]').forEach(radio => {
        radio.addEventListener('change', () => toggleStudentInputs(radio.value));
        if (radio.checked) {
            toggleStudentInputs(radio.value);
        }
    });

    function toggleStudentInputs(selectedValue) {
        const isOtherSchool = selectedValue === '他校學生';
        document.getElementById('currentSchool').disabled = !isOtherSchool;
        document.getElementById('department').disabled = !isOtherSchool;
        if (isOtherSchool) document.getElementById('currentSchool').focus();

        if (isOtherSchool) {
            updateStudyStatusMemo_otherSchool();
            document.getElementById('currentSchool').addEventListener('input', updateStudyStatusMemo_otherSchool);
            document.getElementById('department').addEventListener('input', updateStudyStatusMemo_otherSchool);
        } else {
            document.getElementById('studyStatusMemo').value = ''; // 清空 studyStatusMemo
        }

        const isNonStudent = selectedValue === '非在學學生';
        document.getElementById('hasOtherJob').disabled = !isNonStudent;
        document.getElementById('noOtherJob').disabled = !isNonStudent;

        // 更新 studyStatusMemo 的值
        if (isNonStudent) {
            updateStudyStatusMemo_nonStudent();
        } else {
            document.getElementById('hasOtherJob').checked = false;
            document.getElementById('noOtherJob').checked = false;
            document.getElementById('studyStatusMemo').value = ''; // 清空 studyStatusMemo
        }
    }
    function updateStudyStatusMemo_otherSchool() {
        const currentSchool = document.getElementById('currentSchool').value;
        const department = document.getElementById('department').value;

        let memoContent;

        if (currentSchool) {
            memoContent = `${currentSchool}`;
        }

        if (department) {
            memoContent += `/${department}`;
        }

        document.getElementById('studyStatusMemo').value = memoContent;
    }


    function updateStudyStatusMemo_nonStudent() {
        const hasOtherJob = document.getElementById('hasOtherJob').checked;
        const noOtherJob = document.getElementById('noOtherJob').checked;


        if (hasOtherJob) {
            document.getElementById('studyStatusMemo').value = '有其他專職';
        } else if (noOtherJob) {
            document.getElementById('studyStatusMemo').value = '無其他專職';
        } else {
            document.getElementById('studyStatusMemo').value = '';
        }
    }

    // 監聽 hasOtherJob 和 noOtherJob 的变化
    document.getElementById('hasOtherJob').addEventListener('change', updateStudyStatusMemo_nonStudent);
    document.getElementById('noOtherJob').addEventListener('change', updateStudyStatusMemo_nonStudent);

    // 初始化时檢查被選中的薪水類型
    const selectedCategory = document.querySelector('input[name="SalaryCategory"]:checked');
    if (selectedCategory) {
        toggleSalaryInput(selectedCategory.value);
    }

    document.querySelectorAll('input[name="SalaryCategory"]').forEach(radio => {
        radio.addEventListener('change', () => toggleSalaryInput(radio.value));
    });

    function toggleSalaryInput(selectedValue) {
        ['monthlySalaryAmount', 'dailySalaryAmount', 'hourlySalaryAmount'].forEach(id => {
            const input = document.getElementById(id);
            if (input) {
                input.disabled = selectedValue !== getInputTypeFromId(id);
                if (input.disabled) input.value = ''; // 清空未選中輸入框的值
            }
        });
    }

    function getInputTypeFromId(id) {
        switch (id) {
            case 'monthlySalaryAmount': return '月薪';
            case 'dailySalaryAmount': return '日薪';
            case 'hourlySalaryAmount': return '時薪';
            default: return '';
        }
    }

    
});


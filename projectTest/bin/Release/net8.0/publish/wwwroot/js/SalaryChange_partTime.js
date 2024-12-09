document.addEventListener('DOMContentLoaded', function () {
    // 重置表單時恢復禁用
    document.getElementById('Requisition').addEventListener('reset', function () {
        const idsToDisable = [
            'monthlySalaryAmount',
            'dailySalaryAmount',
            'hourlySalaryAmount',            
        ];
        idsToDisable.forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.disabled = true;
            }
        });
    });          
 
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


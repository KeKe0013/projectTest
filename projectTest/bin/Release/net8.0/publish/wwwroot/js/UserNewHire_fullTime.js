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
        const day = parseInt(dateString.substr(5, 2));
        const dateObj = new Date(year, month, day);
        document.getElementById(fieldName + 'Input').value = dateObj.toISOString().split('T')[0];
        document.getElementById(fieldName + 'Hidden').value = dateString;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.date-input').forEach(input => {
        input.addEventListener('change', () => updateHiddenField(input));
        updateHiddenField(input); 
    });

    ['birthday', 'employedStart', 'employedEnd'].forEach(field => {
        initializeDateField(field, document.getElementById(field + 'Hidden')?.value);
    });
});

let planCount = 1;

async function addPlanInfo() {
    if (planCount >= 5) {
        alert("最多只能新增五組計畫資料");
        return;
    }

    planCount++;
    const planInfoContainer = document.createElement("div");
    planInfoContainer.classList.add("plan-information", "extra-plan");

    const response = await fetch(`/NewHire/RenderAddPlanView?index=${planCount}`);
    if (response.ok) {
        const html = await response.text();
        planInfoContainer.innerHTML = html.replace(/__index__/g, planCount);

        const newDateInputs = planInfoContainer.querySelectorAll('.date-input');
        newDateInputs.forEach(input => {
            input.addEventListener('change', () => updateHiddenField(input));
            updateHiddenField(input);
        });

        document.querySelector(".Application-form form").insertBefore(
            planInfoContainer,
            document.querySelector(".person-information")
        );
    } else {
        console.error("無法載入新增計畫的視圖內容");
    }
}

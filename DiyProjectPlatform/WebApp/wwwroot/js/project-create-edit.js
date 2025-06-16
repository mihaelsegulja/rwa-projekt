const images = [];
const maxSize = 1024 * 1024;

function toBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(",")[1]);
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });
}

$(function () {
    const $imageInput = $('#imageInput');
    const $previewList = $('#imagePreviewList');

    $imageInput.on('change', async function () {
        $previewList.empty();
        images.length = 0;

        const files = Array.from(this.files);

        for (let index = 0; index < files.length; index++) {
            const file = files[index];

            if (file.size > maxSize) {
                showToast(`File "${file.name}" is too large (max 1MB)`, "error");
                continue;
            }

            const base64 = await toBase64(file);
            const isFirst = index === 0;

            const imgObj = {
                ImageData: base64,
                Description: "",
                IsMainImage: isFirst
            };
            images.push(imgObj);

            const cardHtml = `
                        <div class="mb-3">
                            <div class="card">
                                <div class="card-body d-flex align-items-center">
                                    <img src="data:image/png;base64,${base64}" alt="Preview" class="me-3" style="width: 80px; height: 80px; object-fit: cover; border-radius: 5px;">
                                    <div style="flex: 1;">
                                        <p class="mb-1">${file.name}</p>
                                        <input type="text" placeholder="Description" class="form-control mb-2 description-input" />
                                        <div class="form-check">
                                            <input type="radio" name="mainImage" class="form-check-input main-image-radio" ${isFirst ? "checked" : ""} />
                                            <label class="form-check-label">Set as main image</label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`;

            $previewList.append(cardHtml);
        }
    });

    $('form').on('submit', function (e) {
        const anyChecked = $('input[name="SelectedMaterialIds"]:checked').length > 0;
        if (!anyChecked) {
            e.preventDefault();
            showToast("Please select at least one material", "error");
            return;
        }

        let isValid = true;

        $('.description-input').each(function (i) {
            const val = $(this).val()?.trim();
            if (!val) {
                isValid = false;
                return false;
            }
            images[i].Description = val;
        });

        if (!isValid) {
            e.preventDefault();
            showToast("Image descriptions are required", "error");
            return;
        }

        $('.main-image-radio').each(function (i) {
            images[i].IsMainImage = $(this).is(':checked');
        });

        $('#ImagesJson').val(JSON.stringify(images));
    });
});

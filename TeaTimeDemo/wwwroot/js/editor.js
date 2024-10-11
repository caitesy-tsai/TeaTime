// 等待文檔加載完成後執行
$(document).ready(function () {
    initializeTinyMCE();
    initializeQuestionIndices();
});

// 初始化 TinyMCE 編輯器
function initializeTinyMCE() {
    tinymce.init({
        selector: '#editor',
        api_key: 'bd4kr41e6ze0pbf2aykxdz4hsbpnedbrhpjj227b6za85wou', // 替換為您的 API 金鑰
        plugins: 'advlist autolink lists link image charmap preview anchor code',
        toolbar: 'undo redo | formatselect | bold italic backcolor | code | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | removeformat',
        height: '100%', // 設置高度為 100%
        setup: function (editor) {
            editor.on('init', function () {
                var initialContent = $('#MceHtml').val();
                editor.setContent(initialContent);
            });

            editor.on('change keyup', function () {
                tinymce.triggerSave();
                $('#MceHtml').val(editor.getContent());
            });
        },
        content_style: "body { font-size:14px; }" // 根據需要調整編輯器內的字體大小
    });
}

// 初始化問題和選項的索引
function initializeQuestionIndices() {
    window.questionIndex = window.initialData.questionCount || 0;
    window.optionsIndices = window.initialData.optionsIndices || {};
}

// 更新 TinyMCE 編輯器的內容
function updateMceContent() {
    var content = generateMceContent();
    $('#MceHtml').val(content);
    tinymce.get('editor').setContent(content);
}

// 生成 TinyMCE 編輯器的內容
function generateMceContent() {
    var title = $('input[name="Survey.Title"]').val();
    var description = $('textarea[name="Survey.Description"]').val();
    var stationName = $('select[name="Survey.StationName"] option:selected').text();
    var questionNum = $('input[name="Survey.QuestionNum"]').val();

    var surveyImagesHtml = '';
    $('#survey-image-preview .mb-3 img').each(function () {
        var imgSrc = $(this).attr('src');
        surveyImagesHtml += '<img src="' + imgSrc + '" alt="問卷圖片" style="max-width: 200px;" /><br />';
    });

    var questionsHtml = '';
    $('#questions .question-container').each(function () {
        var questionText = $(this).find('input[name$=".Question.QuestionText"]').val();
        var answerTypeValue = $(this).find('select[name$=".Question.AnswerType"]').val();
        var answerType = $(this).find('select[name$=".Question.AnswerType"] option:selected').text();
        var optionsHtml = '';

        var qIndex = $(this).attr('id').split('-')[1];

        $(this).find('.option-container').each(function () {
            var optionText = $(this).find('input[name$=".QuestionOption.OptionText"]').val();
            var optionHtml = '';

            switch (answerType.toLowerCase()) {
                case '單選':
                case 'radio':
                    var optionId = $(this).find('input[name$=".QuestionOption.Id"]').val();
                    optionHtml += '<input type="radio" name="Questions[' + qIndex + '].SelectedOption" value="' + optionId + '" id="option_' + optionId + '" required> ' +
                        '<label for="option_' + optionId + '">' + optionText + '</label><br />';
                    break;
                case '多選':
                case 'checkbox':
                    var optionId = $(this).find('input[name$=".QuestionOption.Id"]').val();
                    optionHtml += '<input type="checkbox" name="Questions[' + qIndex + '].SelectedOptions" value="' + optionId + '" id="option_' + optionId + '"> ' +
                        '<label for="option_' + optionId + '">' + optionText + '</label><br />';
                    break;
                case '填空':
                case 'text':
                    optionHtml += '<input type="text" name="Questions[' + qIndex + '].AnswerText" required class="form-control" /><br />';
                    break;
                case '填空框':
                case 'textarea':
                    optionHtml += '<textarea name="Questions[' + qIndex + '].AnswerText" required class="form-control"></textarea><br />';
                    break;
                case '下拉選單':
                case 'select':
                    optionHtml += '<select name="Questions[' + qIndex + '].AnswerText" class="form-select"><option>' + optionText + '</option></select><br />';
                    break;
                default:
                    optionHtml += optionText + '<br />';
                    break;
            }

            var optionIdPreview = $(this).find('input[name$=".QuestionOption.Id"]').val();
            var optionImagesHtml = '';
            $(this).find('#option-image-preview-' + qIndex + '-' + optionIdPreview + ' .mb-3 img').each(function () {
                var imgSrc = $(this).attr('src');
                optionImagesHtml += '<img src="' + imgSrc + '" alt="選項圖片" style="max-width: 200px;" /><br />';
            });

            if (optionImagesHtml) {
                optionHtml += '<br />' + optionImagesHtml;
            }

            optionsHtml += optionHtml;
        });

        var questionImagesHtml = '';
        $(this).find('.question-image-preview img').each(function () { // 修改選擇器
            var imgSrc = $(this).attr('src');
            questionImagesHtml += '<img src="' + imgSrc + '" alt="問題圖片" style="max-width: 200px;" /><br />';
        });

        var questionHtml = '<h3>' + questionText + ' (' + answerType + ')</h3>';
        if (questionImagesHtml) {
            questionHtml += '<div>' + questionImagesHtml + '</div>';
        }
        if (optionsHtml) {
            questionHtml += '<div>' + optionsHtml + '</div>';
        }
        questionsHtml += questionHtml;
    });

    var content = '<h1>' + title + '<span style="font-size: 0.7em; margin-left: 10px;">' +
        '站別: ' + stationName + ' 頁數：' + questionNum +
        '</span></h1>' +
        '<p>' + description + '</p>';

    if (surveyImagesHtml) {
        content += '<div>' + surveyImagesHtml + '</div>';
    }

    content += questionsHtml;

    return content;
}

// 刪除圖片的前端邏輯，通過 Ajax 請求後端刪除圖片
function removeImage(imageId) {
    $.ajax({
        url: '/admin/survey/removeimage',
        type: 'POST',
        data: { id: imageId },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                $('button[onclick="removeImage(' + imageId + ')"]').closest('.mb-3').remove();
                updateMceContent();
            } else {
                toastr.error(response.message);
            }
        },
        error: function (xhr) {
            console.log(xhr.responseText);
            toastr.error('刪除圖片時發生錯誤');
        }
    });
}

// 新增問題的函數
function addQuestion() {
    var questionHtml = `
        <div class="border p-2 my-3 question-container" id="question-${questionIndex}">
            <h4>問題 ${questionIndex + 1}</h4>
            <input type="hidden" name="QuestionVMs[${questionIndex}].Question.Id" value="0" />
            <div class="mb-3">
                <label class="form-label">問題描述</label>
                <input name="QuestionVMs[${questionIndex}].Question.QuestionText" class="form-control" />
            </div>
            <div class="mb-3">
                <label class="form-label">問題類型</label>
                <select name="QuestionVMs[${questionIndex}].Question.AnswerType" class="form-select" onchange="updateMceContent()">
                    ${generateQuestionTypeOptions()}
                </select>
            </div>
            <!-- 問題圖片預覽區域 -->
            <div class="mb-3 question-image-preview" id="question-image-preview-${questionIndex}">
                <!-- 新增的問題圖片會在這裡預覽 -->
            </div>
            <div class="mb-3">
                <label class="form-label">上傳問題圖片</label>
                <div id="question-image-upload-container-${questionIndex}">
                    <!-- 初始不顯示圖片上傳欄位 -->
                </div>
                <button type="button" class="btn btn-primary" onclick="addQuestionImageUploadField(${questionIndex})">新增圖片+</button>
                <span class="text-danger"></span>
            </div>
            <div id="options-${questionIndex}">
                <!-- 預設沒有選項，使用者可以點擊按鈕新增 -->
            </div>
            <button type="button" class="btn btn-primary" onclick="addOption(${questionIndex})">新增選項</button>
            <button type="button" class="btn btn-danger mt-2" onclick="removeQuestion(${questionIndex}, 0)">刪除問題</button>
        </div>`;
    $('#questions').append(questionHtml);
    questionIndex++;

    // 更新 TinyMCE 編輯器的內容
    updateMceContent();
}

// 生成問題類型的選項 HTML
function generateQuestionTypeOptions() {
    var questionTypeList = window.initialData.questionTypeList;
    var options = '';
    questionTypeList.forEach(function (type) {
        options += `<option value="${type.Value}">${type.Text}</option>`;
    });
    return options;
}

// 刪除問題的函數
function removeQuestion(index, questionId) {
    if (questionId) {
        $.ajax({
            url: '/admin/survey/removequestion',
            type: 'POST',
            data: { id: questionId },
            success: function (response) {
                if (response.success) {
                    $('#question-' + index).remove();
                    toastr.success(response.message);
                    reindexQuestions();
                    updateMceContent();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('刪除問題時發生錯誤');
            }
        });
    } else {
        $('#question-' + index).remove();
        reindexQuestions();
        updateMceContent();
    }
}

// 重新整理問題索引的函數
function reindexQuestions() {
    $('#questions .question-container').each(function (index) {
        $(this).attr('id', `question-${index}`);
        $(this).find('h4').text(`問題 ${index + 1}`);
        $(this).find('input, select, textarea').each(function () {
            var name = $(this).attr('name');
            if (name) {
                name = name.replace(/QuestionVMs\[\d+\]/, `QuestionVMs[${index}]`);
                $(this).attr('name', name);
            }
        });
        var questionId = $(this).find('input[type="hidden"]').val();
        $(this).find('.btn-danger').attr('onclick', `removeQuestion(${index}, ${questionId})`);
        reindexOptions(index);
    });
    window.questionIndex = $('#questions .question-container').length;
}

// 新增選項的函數
function addOption(questionIndex) {
    if (!window.optionsIndices[questionIndex]) {
        window.optionsIndices[questionIndex] = 0;
    }
    var optionIndex = window.optionsIndices[questionIndex];
    var optionHtml = `
        <div class="mb-2 option-container" id="option-${questionIndex}-${optionIndex}">
            <input type="hidden" name="QuestionVMs[${questionIndex}].QuestionOptionVMs[${optionIndex}].QuestionOption.Id" value="0" />
            <label class="form-label">選項 ${optionIndex + 1}</label>
            <input name="QuestionVMs[${questionIndex}].QuestionOptionVMs[${optionIndex}].QuestionOption.OptionText" class="form-control" onchange="updateMceContent()" />
            <button type="button" class="btn btn-danger" onclick="removeOption(${questionIndex}, ${optionIndex}, 0)">刪除選項</button>
            <div class="mb-3">
                <label class="form-label">上傳選項圖片</label>
                <div id="option-image-upload-container-${questionIndex}-${optionIndex}">
                    <!-- 初始不顯示圖片上傳欄位 -->
                </div>
                <button type="button" class="btn btn-primary" onclick="addOptionImageUploadField(${questionIndex}, ${optionIndex})">新增圖片+</button>
                <span class="text-danger"></span>
            </div>
            <div class="mb-3 option-image-preview" id="option-image-preview-${questionIndex}-${optionIndex}">
                <!-- 新增的選項圖片會在這裡預覽 -->
            </div>
        </div>`;

    $(`#options-${questionIndex}`).append(optionHtml);
    window.optionsIndices[questionIndex]++;
    updateMceContent();
}

// 刪除選項的函數
function removeOption(questionIndex, optionIndex, optionId) {
    if (optionId) {
        $.ajax({
            url: '/admin/survey/removeoption',
            type: 'POST',
            data: { id: optionId },
            success: function (response) {
                if (response.success) {
                    $(`#option-${questionIndex}-${optionIndex}`).remove();
                    toastr.success(response.message);
                    reindexOptions(questionIndex);
                    updateMceContent();
                } else {
                    toastr.error(response.message);
                }
            },
            error: function () {
                toastr.error('刪除選項時發生錯誤');
            }
        });
    } else {
        $(`#option-${questionIndex}-${optionIndex}`).remove();
        reindexOptions(questionIndex);
        updateMceContent();
    }
}

// 重新整理選項索引的函數
function reindexOptions(questionIndex) {
    var optionContainers = $(`#options-${questionIndex} .option-container`);
    optionContainers.each(function (index) {
        $(this).attr('id', `option-${questionIndex}-${index}`);
        $(this).find('.form-label').text(`選項 ${index + 1}`);
        $(this).find('input, select, textarea').each(function () {
            var name = $(this).attr('name');
            if (name) {
                name = name.replace(/QuestionOptionVMs\[\d+\]/, `QuestionOptionVMs[${index}]`);
                $(this).attr('name', name);
            }
        });
        var optionId = $(this).find('input[type="hidden"]').val();
        $(this).find('.btn-danger').attr('onclick', `removeOption(${questionIndex}, ${index}, ${optionId})`);
    });
    window.optionsIndices[questionIndex] = optionContainers.length;
}

// 新增問卷圖片上傳欄位的函數
function addSurveyImageUploadField() {
    var html = '<div class="mb-2"><input type="file" name="SurveyImageFiles" class="form-control" onchange="previewAndSyncImage(this, \'survey\')" /></div>';
    $('#survey-image-upload-container').append(html);
    updateMceContent();
}

// 新增問題圖片上傳欄位的函數
function addQuestionImageUploadField(questionIndex) {
    var html = `
        <div class="mb-2">
            <input type="file" name="QuestionVMs[${questionIndex}].QuestionImageFiles" class="form-control" onchange="previewAndSyncImage(this, 'question', ${questionIndex})" />
        </div>`;
    $(`#question-image-upload-container-${questionIndex}`).append(html);
    updateMceContent();
}

// 新增選項圖片上傳欄位的函數
function addOptionImageUploadField(questionIndex, optionIndex) {
    var html = `
        <div class="mb-2">
            <input type="file" name="QuestionVMs[${questionIndex}].QuestionOptionVMs[${optionIndex}].OptionImageFiles" class="form-control" onchange="previewAndSyncImage(this, 'option', ${questionIndex}, ${optionIndex})" />
        </div>`;
    $(`#option-image-upload-container-${questionIndex}-${optionIndex}`).append(html);
    updateMceContent();
}

// 當圖片上傳時，更新 TinyMCE 編輯器中的圖片內容
function previewAndSyncImage(input, type, questionIndex, optionIndex) {
    const maxSize = 5 * 1024 * 1024; // 最大5MB
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];

    if (input.files && input.files[0]) {
        const file = input.files[0];

        // 檢查文件類型
        if (!allowedTypes.includes(file.type)) {
            toastr.error('僅允許上傳 JPG、PNG、WEBP 或 GIF 格式的圖片。');
            input.value = ''; // 清空選擇
            return;
        }

        // 檢查文件大小
        if (file.size > maxSize) {
            toastr.error('圖片大小不得超過 5MB。');
            input.value = '';
            return;
        }

        var reader = new FileReader();
        reader.onload = function (e) {
            var previewImage = `
                <div class="mb-3">
                    <img src="${e.target.result}" alt="${type} Image" style="max-width: 200px;" />
                    <button type="button" class="btn btn-danger" onclick="removePreviewImage(this)">刪除</button>
                </div>`;

            if (type === 'survey') {
                $('#survey-image-preview').append(previewImage);
            } else if (type === 'question') {
                $('#question-image-preview-' + questionIndex).append(previewImage);
            } else if (type === 'option') {
                $('#option-image-preview-' + questionIndex + '-' + optionIndex).append(previewImage);
            }

            updateMceContent(); // 更新 MceHtml
        }
        reader.readAsDataURL(file);
    }
}

// 刪除預覽圖片
function removePreviewImage(button) {
    $(button).parent().remove();
    updateMceContent(); // 更新 MceHtml
}

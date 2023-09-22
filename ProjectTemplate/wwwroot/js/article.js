
$(document).ready(function () {
    $("#createArticleBtn").click(function () {
        $.ajax({
            type: "POST",
            url: "/article/create_new_article",
            data: {
                article_type: 1, //hard coded for now
            },
            dataType: "json",
            success: function (data) {
                if (data.articleId) {
                    // Redirect to the edit page of the newly created article.
                    window.location.href = "/article/edit/" + data.articleId;
                } else {
                    alert("Failed to create a new article.");
                }
            },
            error: function () {
                alert("An error occurred while creating a new article.");
            }
        });
    });
});

tinymce.init({
    selector: 'textarea',
    plugins: 'tinycomments',
    toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table mergetags | align lineheight | tinycomments | checklist numlist bullist indent outdent | emoticons charmap | removeformat',
    tinycomments_mode: 'embedded',
    tinycomments_author: 'Author name',
    mergetags_list: [
        { value: 'First.Name', title: 'First Name' },
        { value: 'Email', title: 'Email' },
    ],
    ai_request: (request, respondWith) => respondWith.string(() => Promise.reject("See docs to implement AI Assistant"))
});


function saveArticle() {
    var editorContent = tinymce.get('articleEditor').getContent();
    if (editorContent === "")
        return;
    var comments = "";
    var articleId = getCurrentArticleId();
    //TODO: comments plugin requires discussion related to the selection of mode and saving the comments in database
    //1. Callback mode
    //2. Embedded mode
    //https://www.tiny.cloud/docs/plugins/premium/comments/introduction_to_tiny_comments/

    $.ajax({
        type: "POST",
        url: "/Article/SaveArticle",
        data: { content: editorContent, comments: comments, id: articleId },
        success: function (result) {
            if (result.success) {
                console.log("Content saved successfully.");
                //window.location.href = "/Article/Articles/";
            } else {
                console.error("Failed to save content.");
            }
        },
        error: function () {
            console.error("An error occurred while saving content.");
        }
    });
}

function autoSaveArticle() {
    var editorContent = tinymce.get('articleEditor').getContent();
    if (editorContent === "")
        return;
    var comments = "";
    var articleId = getCurrentArticleId();
    //TODO: comments plugin requires discussion related to the selection of mode and saving the comments in database
    //1. Callback mode
    //2. Embedded mode
    //https://www.tiny.cloud/docs/plugins/premium/comments/introduction_to_tiny_comments/

    $.ajax({
        type: "POST",
        url: "/Article/AutoSaveArticle",
        data: { content: editorContent, comments: comments, id: articleId },
        success: function (result) {
            if (result.success) {
                console.log("Content saved successfully.");
                //window.location.href = "/Article/Articles/";
            } else {
                console.error("Failed to save content.");
            }
        },
        error: function () {
            console.error("An error occurred while saving content.");
        }
    });
}

function autoSaveArticleInterval() {
    autoSaveArticle();
    console.log("Content saved.");
    setTimeout(autoSaveArticleInterval, 2 * 60 * 1000);
}

function getCurrentArticleId() {
    var pathname = window.location.pathname;
    var parts = pathname.split("/");
    var id = parts[parts.length - 1];
    return id;
}

function manualSaveArticle() {
    saveArticle();
}

function cancel() {
    window.location.href = "/article/articles/";
}

function getArticleForm(formData, articleId) {
    let form;
    var existingFormData = formData;

    $.ajax({
        type: "POST",
        url: "/article/get_article_form_definition",
        data: {
            type: 'sport' // hard coded for now
        },
        dataType: "json",
        success: function (data) {
            if (data.articleFormDefinition) {
                Formio.createForm(document.getElementById('formio'), JSON.parse(data.articleFormDefinition))

                    .then(function (createdForm) {
                        form = createdForm;
                        
                        if (existingFormData) {
                            form.submission = {
                                data: JSON.parse(existingFormData)
                            };
                        }

                        form.on('submit', function (submission) {
                            const formDataString = JSON.stringify(submission.data);
                            $.ajax({
                                type: 'POST',
                                url: '/article/save_form_data',
                                data: {
                                    formData: formDataString,
                                    articleId: articleId
                                },
                                dataType: 'json',
                                success: function (response) {
                                    //TODO: Need to do something later
                                },
                                error: function (error) {
                                    console.error('Error submitting form data:', error);
                                }
                            });
                        });
                    })
                    .catch(function (error) {
                        console.error('Error creating the form:', error);
                    });
            }
        }
    });

    document.getElementById('submitButton').addEventListener('click', function () {
        manualSaveArticle()
        form.submit();
        alert('Article and form definition have been saved!');
    });
}
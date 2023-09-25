
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

function tinycomments_create(req, done, fail) {
    var comments = req.content;
    var createdAt = req.createdAt;
    var articleId = $("#articleId").val();

    fetch('/article/create_article_comments', {
        method: 'POST',
        body: JSON.stringify({ Article_Id: parseInt(articleId, 10), Comments: comments, Created_At: createdAt }),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error('Failed to create comment');
            }
            return response.json();
        })
        .then((req2) => {
            let conversationUid = 1;// req2.conversationUid; //TODO: hard coded for now
            done({ conversationUid: conversationUid });
        })
        .catch((e) => {
            fail(e);
        });
}

function tinycomments_edit_comment(req, done, fail) {
    let conversationUid = req.conversationUid;
    let commentUid = req.commentUid;
    let content = req.content;
    let modifiedAt = req.modifiedAt;
    var articleId = $("#articleId").val();

    fetch(
        '/article/edit_article_comments',
        {
            method: 'POST',
            body: JSON.stringify({ Article_Id: parseInt(articleId, 10), Comments: content, Created_At: modifiedAt }),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
        }
    )
        .then((response) => {
            if (!response.ok) {
                throw new Error('Failed to edit comment');
            }
            return response.json();
        })
        .then((req2) => {
            let canEdit = req2.canEdit;
            done({ canEdit: canEdit });
        })
        .catch((e) => {
            fail(e);
        });
}

function tinycomments_lookup({ conversationUid }, done, fail) {
    let lookup = async function () {
        var articleId = $("#articleId").val();
        let convResp = await fetch(
            '/Article/GetArticleComments', {
                method: 'POST',
                body: JSON.stringify({ Article_Id: parseInt(articleId, 10) }),
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
        }// + conversationUid
        );
        if (!convResp.ok) {
            throw new Error('Failed to get conversation');
        }
        let comments = await convResp.json();
        //let usersResp = await fetch('/Article/GetArticleUsers');
        //if (!usersResp.ok) {
        //    throw new Error('Failed to get users');
        //}
        //let { users } = await usersResp.json();
        //let getUser = function (userId) {
        //    return users.find((u) => {
        //        return u.id === userId;
        //    });
        //};
        return {
            conversation: {
                uid: 1, //conversationUid, //TODO: hard coded for now
                comments: comments.map((comment) => {
                    return {
                        ...comment,
                        content: comment.comments,
                        authorName: 'test_author' //getUser(comment.author)?.displayName, //TODO: need discussion on the author management, hard coded for now
                    };
                }),
            },
        };
    };
    lookup()
        .then((data) => {
            console.log('Lookup success ' + conversationUid, data);
            done(data);
        })
        .catch((err) => {
            console.error('Lookup failure ' + conversationUid, err);
            fail(err);
        });
}

tinymce.init({
    selector: 'textarea#articleEditor',
    height: 800,
    plugins: 'code tinycomments help lists',
    toolbar:
        'undo redo | formatselect | ' +
        'bold italic backcolor | alignleft aligncenter ' +
        'alignright alignjustify | bullist numlist outdent indent | ' +
        'removeformat | addcomment showcomments | help',
    menubar: 'file edit view insert format tc',
    menu: {
        tc: {
            title: 'Comments',
            items: 'addcomment showcomments deleteallconversations',
        },
    },
    tinycomments_create,
    tinycomments_reply,
    tinycomments_edit_comment,
    tinycomments_delete,
    tinycomments_delete_all,
    tinycomments_delete_comment,
    tinycomments_lookup,
    tinycomments_author: 'test_author', //TODO: need discussion on the author management, hard coded for now
    setup: function (editor) {
        editor.on('SkinLoaded', () => {
            editor.execCommand('ToggleSidebar', false, 'showcomments');
        });
    },
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
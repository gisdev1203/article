/********************************
 *   Tiny Comments functions    *
 * (must call "done" or "fail") *
 ********************************/

const tinycomments_create = (req, done, fail) => {
    const { content, createdAt } = req;
    var id_article = $("#id_article").val();
    let id_conversation = generateUUID();

    fetch('/article/create_article_comments', {
        method: 'POST',
        body: JSON.stringify({ Id_article: parseInt(id_article, 10), Comments: content, Created_At: createdAt, Id_conversation: id_conversation, Modified_at: createdAt }),
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
            //const conversationUid = req2.conversationUid;
            done({ conversationUid: id_conversation });
        })
        .catch((e) => {
            fail(e);
        });
};

const tinycomments_reply = (req, done, fail) => {
    const { conversationUid, content, createdAt } = req;
    let id_comment = generateUUID();
    var id_article = $("#id_article").val();

    fetch(`/article/reply_article_comments`, {
        method: 'POST',
        body: JSON.stringify({ Comments: content, Created_At: createdAt, Modified_at: createdAt, Id_conversation: conversationUid, Id_article: parseInt(id_article, 10), Id_comment: id_comment }),
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
        },
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error('Failed to reply to comment');
            }
            return response.json();
        })
        .then((ref2) => {
            done({ commentUid: id_comment });
        })
        .catch((e) => {
            fail(e);
        });

};

const tinycomments_edit_comment = (req, done, fail) => {
    const { conversationUid, commentUid, content, modifiedAt } = req;
    var id_article = $("#id_article").val();
    fetch(
        '/article/edit_article_comments',
        {
            method: 'POST',
            body: JSON.stringify({ Id_article: parseInt(id_article, 10), Comments: content, Modified_At: modifiedAt, Id_conversation: conversationUid, Id_comment: commentUid }),
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

            done({ canEdit: true });
        })
        .catch((e) => {
            fail(e);
        });
};

const tinycomments_delete = (req, done, fail) => {
    const conversationUid = req.conversationUid;
    var id_article = $("#id_article").val();

    fetch('/article/delete_article_conversation', {
        method: 'POST',
        body: JSON.stringify({ Id_article: parseInt(id_article, 10), Id_conversation: conversationUid }),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    }).then((response) => {
        if (response.ok) {
            done({ canDelete: true });
        } else if (response.status === 403) {
            done({ canDelete: false });
        } else {
            fail(new Error('Something has gone wrong...'));
        }
    });    
};

const tinycomments_delete_all = (_req, done, fail) => {
    fetch('https://api.example/conversations', {
        method: 'DELETE',
    }).then((response) => {
        if (response.ok) {
            done({ canDelete: true });
        } else if (response.status === 403) {
            done({ canDelete: false });
        } else {
            fail(new Error('Something has gone wrong...'));
        }
    });
};

const tinycomments_delete_comment = (req, done, fail) => {
    const { conversationUid, commentUid } = req;
    var id_article = $("#id_article").val();

    fetch('/article/delete_article_comments', {
        method: 'POST',
        body: JSON.stringify({ Id_article: parseInt(id_article, 10), Id_conversation: conversationUid, Id_comment: commentUid }),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        },
    })
        .then((response) => {
            if (response.ok) {
                done({ canDelete: true });
            } else if (response.status === 403) {
                done({ canDelete: false });
            } else {
                fail(new Error('Something has gone wrong...'));
            }
        });
};

const tinycomments_lookup = ({ conversationUid }, done, fail) => {
    const lookup = async () => {
        var id_article = $("#id_article").val();

        const convResp = await fetch(
            '/article/GetArticleComments/', {
            method: 'POST',
            body: JSON.stringify({ Id_article: parseInt(id_article, 10), Id_conversation: conversationUid }),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
        }
        );
        if (!convResp.ok) {
            throw new Error('Failed to get conversation');
        }
        const comments = await convResp.json();

        return {
            conversation: {
                uid: conversationUid,
                comments: comments.map((comment) => ({
                    ...comment,
                    createdAt: comment.created_at,
                    content: comment.comments,
                    modifiedAt: comment.modified_at,
                    uid: comment.id_comment,
                    authorName: "test_User",
                    commentUid: comment.id_comment,
                })),
            },
        };
    };
    lookup()
        .then((data) => {            
            done(data);
        })
        .catch((err) => {
            console.error('Lookup failure ' + conversationUid, err);
            fail(err);
        });
};

tinymce.init({
    selector: 'textarea#comments-callback',
    language: 'fr_FR',
    height: 800,
    plugins: 'code tinycomments help lists',
    toolbar:
        'undo redo | blocks | ' +
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
    tinycomments_mode: 'callback',
    tinycomments_create,
    tinycomments_reply,
    tinycomments_edit_comment,
    tinycomments_delete,
    tinycomments_delete_all,
    tinycomments_delete_comment,
    tinycomments_lookup,

    setup: (editor) => {
        editor.on('SkinLoaded', () => {
            editor.execCommand('ToggleSidebar', false, 'showcomments');
        });
    },
});

//We can use JavaScript's UUID package, but for now below solution is more robust! 
function generateUUID() {
    let dt = new Date().getTime();
    const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = (dt + Math.random() * 16) % 16 | 0;
        dt = Math.floor(dt / 16);
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
    return uuid;
}


function saveArticle() {
    var editorContent = tinymce.get('comments-callback').getContent();
    if (editorContent === "")
        return;
    
    var id_article = $("#id_article").val();
    var formio_data = $("#formio_data").val();

    $.ajax({
        type: "POST",
        url: "/Article/SaveArticle",
        data: { content: editorContent, formio_data: formio_data, id: id_article },
        success: function (result) {
            if (!result.success) {
                console.error("Failed to save content.");
            } else {
                window.location.href = "/article/articles/";
            }
        },
        error: function () {
            console.error("An error occurred while saving content.");
        }
    });
}

function autoSaveArticle() {
    var editorContent = tinymce.get('comments-callback').getContent();
    if (editorContent === "")
        return;   
    var id_article = $("#id_article").val();

    $.ajax({
        type: "POST",
        url: "/article/AutoSaveArticle",
        data: { content: editorContent, id: id_article },
        success: function (result) {
            if (!result.success) {                
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
    setTimeout(autoSaveArticleInterval, 1 * 60 * 1000);
}

function cancel() {
    window.location.href = "/article/articles/";
}

function getArticleForm(formData, article_type) {
    var existingFormData = formData;
    $.ajax({
        type: "POST",
        url: "/article/get_article_form_definition",
        data: {
            id: article_type
        },
        dataType: "json",
        success: function (data) {
            if (data.articleFormDefinition) {
                Formio.createForm(document.getElementById('formio'), JSON.parse(data.articleFormDefinition))
                    .then(function (createdForm) {
                        if (existingFormData) {
                            createdForm.submission = {
                                data: JSON.parse(existingFormData)
                            };
                        }
                        createdForm.on('change', function (submission) {
                            $("#formio_data").val(JSON.stringify(submission.data, null, 4))
                        });
                    })
                    .catch(function (error) {
                        console.error('Error creating the form:', error);
                    });
            }
        }
    });
}

$("#submitButton").click(function () {
    saveArticle();
})




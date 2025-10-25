document.addEventListener('DOMContentLoaded', () => {
    class Comment {
        author;
        comment;
        sent;
        articleTitle;
        articleUrl;
    }
    class CommentList {
        comments;
    }
    var forms = document.querySelectorAll('form[data-comment-url]');
    forms.forEach(form => {
        var inputAuthor = form.querySelector('.comment_author');
        var inputComment = form.querySelector('textarea');
        var listKey = "Comments: " + form.dataset.commentUrl;
        var list = JSON.parse(localStorage.getItem(listKey));
        if (list == null)
            list = { comments: [] };
        var next = document.querySelector(".next-comment[data-comment-url=\"" + form.dataset.commentUrl + "\"]");
        list.comments.forEach(showComment);
        function showComment(c) {
            var section = document.createElement("section");
            section.classList.add("comment");
            var p = document.createElement("p");
            p.innerText = c.comment;
            section.append(p);
            var meta = document.createElement("div");
            meta.classList.add("meta");
            section.append(meta);
            var author = document.createElement("span");
            author.textContent = c.author;
            meta.append(author);
            if (c.sent != true) {
                var edit = document.createElement("button");
                edit.type = "button";
                edit.textContent = "Edit Draft";
                edit.addEventListener("click", () => editComment(c, section));
                var send = document.createElement("button");
                send.type = "button";
                send.textContent = "Send comment";
                send.addEventListener("click", () => sendComment(c, section));
                meta.append(edit, send);
            }
            next.parentElement.insertBefore(section, next);
            return section;
        }
        function editComment(c, div) {
            inputComment.value = c.comment;
            inputAuthor.value = c.author;
            div.remove();
            inputComment.scrollIntoView({
                behavior: "smooth",
                block: "center"
            });
        }
        function sendComment(c, div) {
            c.articleTitle = form.dataset.commentTitle;
            c.articleUrl = form.dataset.commentUrl;
            var url = "comment/send/#" + encodeURI(JSON.stringify(c));
            window.open(url);
            c.sent = true;
            localStorage.setItem(listKey, JSON.stringify(list));
        }
        var draftKey = "Comment DRAFT:" + form.dataset.commentUrl;
        var authorKey = "Comment Author";
        try {
            inputAuthor.value = localStorage.getItem(authorKey);
            var draft = JSON.parse(localStorage.getItem(draftKey));
            inputComment.value = draft.comment;
            inputAuthor.value = draft.author;
        }
        catch (ex) {
            console.error(ex);
        }
        inputAuthor.addEventListener("input", saveDraft);
        inputComment.addEventListener("input", saveDraft);
        function saveDraft(ev) {
            var c = {
                author: inputAuthor.value,
                comment: inputComment.value,
            };
            localStorage.setItem(draftKey, JSON.stringify(c));
            localStorage.setItem(authorKey, c.author);
        }
        form.addEventListener('submit', previewComment);
        function previewComment(ev) {
            ev.preventDefault();
            var c = {
                author: inputAuthor.value,
                comment: inputComment.value,
            };
            inputComment.value = "";
            list.comments.push(c);
            localStorage.setItem(listKey, JSON.stringify(list));
            var div = showComment(c);
            div.scrollIntoView({
                behavior: "smooth",
                block: "center"
            });
        }
        form.style.display = "contents";
    });
});

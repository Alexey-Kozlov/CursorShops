$(document).ready(function () {
        //инициализация едитора
        $('#Message').jqxEditor({
            height: 200,
            width: document.querySelector('#CommentArea').style.width - 100,
            localization: {
                "bold": "Жирный шрифт",
                "italic": "Наклонный шрифт",
                "underline": "Подчеркивание",
                "format": "Заголовки",
                "font": "Шрифт",
                "size": "Размер шрифта",
                "color": "Цвет шрифта",
                "background": "Цвет фона",
                "left": "Выравнивание по левому краю",
                "center": "Выравнивание по центру",
                "right": "Выравнивание по правому краю",
                "outdent": "Уменьшить отступ",
                "indent": "Увеличить отступ",
                "ul": "Форматировать как обычный список",
                "ol": "Форматировать как упорядоченный список",
                "html": "Просмотреть текст как HTML"
            },
            tools: "bold italic underline | format font size | color background | left center right | outdent indent | ul ol | html"

        });
        $('#EditForm').jqxValidator({
            //hintType: 'label',
            hintType: 'tooltip',
            position: 'center',
            animationDuration: 300,
            rules: [{
                input: '#Message',
                message: 'Необходимо указать комментарий!',
                action: 'keyup',
                rule: function (input, commit) {
                    var editorValue = $.trim($(input.val()).text());
                    if (editorValue == "" || editorValue == '' || editorValue == "​") {
                        return false;
                    } else {
                        return true;
                    }
                }
            }]
        });

});


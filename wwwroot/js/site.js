$(function () {
    $(".delete-item").click(function (e) {
        confirm("Are you sure want delete this role ?");
        e.preventDefault();
        const anchor = $(this);
        const url = $(anchor).attr("href");
        $.ajax({
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            processData: false,
            type: "DELETE",
            url: url,
            success: function () {
                $(anchor).parent("td").parent("tr").fadeOut("slow",
                    function () {
                        $(this).remove();
                    });
            },
            error: function (xmlHttpRequest, textStatus, errorThrown) {
                let message = `${textStatus} ${xmlHttpRequest.status} ${errorThrown}`;
                if (xmlHttpRequest.responseText !== null) {
                    const response = JSON.parse(xmlHttpRequest.responseText);
                    for (let error of response["Error"]) {
                        message += `\n${error}`;
                    }
                }

                alert(message);
            }
        });
    });

    $("#tree").bonsai({
        expandAll: true,
        checkboxes: true,
        createInputs: "checkbox"
    });

    $("#rastitree").bonsai({
        expandAll: false,
        checkboxes: true,
        createInputs: "checkbox"
    });

    $("#subcontroller").bonsai({
        expandAll: false,
        checkboxes: true,
        createInputs: "checkbox"
    });
   

    $("form").submit(function () {

        //Yleiset
        let controllerIndex = 0, actionIndex = 0;
        $('.controller > input[type="checkbox"]:checked, .controller > input[type="checkbox"]:indeterminate').each(function () {
            const controller = $(this);
            if ($(controller).prop("indeterminate")) {
                $(controller).prop("checked", true);
            }
            const controllerName = "SelectedControllers[" + controllerIndex + "]";
            $(controller).prop("name", controllerName + ".Name");

            const area = $(controller).next().next();
            $(area).prop("name", controllerName + ".AreaName");

            $('ul > li > input[type="checkbox"]:checked', $(controller).parent()).each(function () {
                const action = $(this);
                const actionName = controllerName + ".Actions[" + actionIndex + "].Name";
                $(action).prop("name", actionName);
                actionIndex++;
            });
            actionIndex = 0;
            controllerIndex++;
        });

        //rastit
        let RastiControllerIndex = 0;
        $('.rasticontroller > input[type="checkbox"]:checked, .rasticontroller > input[type="checkbox"]:indeterminate').each(function () {
            
            const controller1 = $(this);
            if ($(controller1).prop("indeterminate")) {
                $(controller1).prop("checked", true);
            }

            //nimi ja id
            const controllerName1 = "ValitutRastit[" + RastiControllerIndex + "]";
            $(controller1).prop("name", controllerName1 + ".Name");
            const area1 = $(controller1).next().next();
            $(area1).prop("name", controllerName1 + ".RastiId");

            //yleiset rastista
            staticrastiactionIndex = 0;
            $(this.parentElement).find('.commonactions > input[type="checkbox"]:checked, .commonactions > input[type="checkbox"]:indeterminate').each(function () {
                
                const controller = $(this);
                if ($(controller).prop("indeterminate")) {
                    $(controller).prop("checked", true);
                }
                const controllerName = "ValitutRastit[" + RastiControllerIndex + "]";

                $('ul > li > input[type="checkbox"]:checked', $(controller).parent()).each(function () {
                    const action = $(this);
                    const actionName = controllerName + ".Actions[" + staticrastiactionIndex + "].Name";
                    $(action).prop("name", actionName);
                    staticrastiactionIndex++;
                });
                staticrastiactionIndex = 0;
                
            });

            //subcontrollerit
            let subcontrollerindex = 0;
            $(this.parentElement).find('.subcontroller > input[type="checkbox"]:checked, .subcontroller > input[type="checkbox"]:indeterminate').each(function () {
                
                const controller = $(this);
                if ($(controller).prop("indeterminate")) {
                    $(controller).prop("checked", true);
                }
                const controllerName = "ValitutRastit[" + RastiControllerIndex + "].SubControllers[" + subcontrollerindex + "]";
                $(controller).prop("name", controllerName + ".Name");

              
                let rastiactionIndex = 0;
                $('ul > li > input[type="checkbox"]:checked', $(controller).parent()).each(function () {
                    const action = $(this);
                    const actionName = controllerName + ".Actions[" + rastiactionIndex + "].Name";
                    $(action).prop("name", actionName);
                    rastiactionIndex++;
                });
               
                subcontrollerindex++;
                
                
            });

            RastiControllerIndex++;
            
        });

       

        return true;
    });
});
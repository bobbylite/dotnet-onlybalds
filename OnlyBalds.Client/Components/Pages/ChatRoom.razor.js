export function initializeChatroomReferences(dotnetHelper) {
    $(document).ready(function() {
        $('.navbar-collapse').collapse('hide');
    });

    dotnetHelper.invokeMethodAsync('Connect');
  };

export function OnEnterKeyPressed(dotnetHelper, user, message) {
    $(document).ready(function() {
        $(document).keypress(function(e) {
            if (e.which == 13) {
                $("input").blur();
                dotnetHelper.invokeMethodAsync('Send', user, message);
            }
        });
    });
}
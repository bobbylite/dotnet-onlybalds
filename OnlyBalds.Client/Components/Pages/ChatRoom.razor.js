/**
 * Handles the on click event for the navbar collapse
 */
export function collapseNavbar() {
    $(document).ready(function() {
        $('.navbar-collapse').collapse('hide');
    });
  };

/**
 * Hanldes the on enter key pressed event
 * @param {*} dotnetHelper Object reference for the dotnet object that invokes the method
 * @param {*} user User that sent the message
 * @param {*} message conent of the message
 */
export const handleOnSendOrEnterKeyPressed = async (dotnetHelper) => {
    $('#send-button').click(async event => {
        var userMessage = $('#message-to-send').val();
        await dotnetHelper.invokeMethodAsync('Send', userMessage);
        $('#message-to-send').val('');

        event.preventDefault();
    });
    $(document).keypress(async event => {
        var userMessage = $('#message-to-send').val();
        if (event.which === 13) {
            await dotnetHelper.invokeMethodAsync('Send', userMessage);
            $('#message-to-send').val('');

            event.preventDefault();
        }
    });
}
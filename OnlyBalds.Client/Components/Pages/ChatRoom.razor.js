/**
 * Handles the on click event for the navbar collapse
 */
export const onRenderAsync = async () => {
    $(document).ready(function() {
        $('.navbar-collapse').collapse('hide');
    });

    $('#send-button').prop('disabled', true);
  };

/**
 * Hanldes the on enter key pressed event
 * @param {*} dotnetHelper Object reference for the dotnet object that invokes the method
 * @param {*} user User that sent the message
 * @param {*} message conent of the message
 */
export const handleOnSendOrEnterKeyPressedAsync = async (dotnetHelper) => {
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

/**
 * Handle the onChatHubConnected event
 * @param {*} dotnetHelper Object reference for the dotnet object that invokes the method
 */
export const onChatHubConnected = async (dotnetHelper) => {
    $('#send-button').prop('disabled', false);
}
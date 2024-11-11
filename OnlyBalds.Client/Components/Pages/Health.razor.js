/**
 * Summary: This method is called when the page is rendered.
 **/
export const onRender = (dotnetHelper, serializedHealthCheck) => {
    $(document).ready(function() {
        $('.navbar-collapse').collapse('hide');

        /**
        * Summary: Rendering Health Check on the page.
        **/
        var healthCheck = JSON.parse(serializedHealthCheck);
        console.log(healthCheck);

        if (healthCheck.status === "Healthy") {
            $('#healthCheckStatus').addClass('alert-success');
        }
        if (healthCheck.status === "Degraded") {
            $('#healthCheckStatus').addClass('alert-warning');
        }
        if (healthCheck.status === "Unhealthy") {
            $('#healthCheckStatus').addClass('alert-danger');
        }

        $('#status').text(healthCheck.status);
        $('#startTime').text(healthCheck.totalDuration);
        generateHealthCheckList(healthCheck);

        /**
         * Summary: This method is called when the user clicks the refresh button.
         **/
        $('#refresh-health-check').click(async function() {
            $(this).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Checking...');
            
            let response = await dotnetHelper.invokeMethodAsync('GetHealthCheckAsync');
            let healthCheck = JSON.parse(response);
            if (healthCheck.status === "Healthy") {
                $('#healthCheckStatus').addClass('alert-success');
                $('#healthCheckStatus').removeClass('alert-danger');
                $('#healthCheckStatus').removeClass('alert-warning');
            }
            if (healthCheck.status === "Degraded") {
                $('#healthCheckStatus').removeClass('alert-success');
                $('#healthCheckStatus').removeClass('alert-danger');
                $('#healthCheckStatus').addClass('alert-warning');
            }
            if (healthCheck.status === "Unhealthy") {
                $('#healthCheckStatus').removeClass('alert-success');
                $('#healthCheckStatus').addClass('alert-danger');
                $('#healthCheckStatus').removeClass('alert-warning');
            }

            $('#status').text(healthCheck.status);
            $('#startTime').text(healthCheck.totalDuration);
            generateHealthCheckList(healthCheck);

            $('#refresh-health-check').html('Refresh');
        });

        // Function to generate the badge based on the status
        function generateBadge(status) {
            let badgeClass = '';
            let badgeText = '';

            if (status === "Healthy") {
                badgeClass = 'badge bg-success';
                badgeText = 'Healthy';
            } else if (status === "Degraded") {
                badgeClass = 'badge bg-warning text-dark';
                badgeText = 'Degraded';
            } else if (status === "Unhealthy") {
                badgeClass = 'badge bg-danger';
                badgeText = 'Unhealthy';
            }

            return `<span class="${badgeClass}">${badgeText}</span>`;
        }

        // Function to generate list items based on the response
        function generateHealthCheckList(response) {
            const entries = response.entries;
            const $listContainer = $('#healthCheckList');
        
            // Clear the container before populating it
            $listContainer.empty();
        
            // Iterate over the entries using Object.entries
            Object.entries(entries).forEach(([key, value]) => {
                const status = value.status;
                const badge = generateBadge(status);
        
                // Create a new list item with jQuery
                const $listItem = $('<li></li>').addClass('list-group-item d-flex justify-content-between align-items-center');
        
                // Set the content (name of the entry and badge)
                $listItem.html(`
                    ${key}
                    ${badge}
                `);
        
                // Append the list item to the list container
                $listContainer.append($listItem);
            });
        }        
    });
}
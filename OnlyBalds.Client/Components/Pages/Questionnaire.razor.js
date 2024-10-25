/**
 * Summary: This method is called when the page is rendered.
 **/
export const onRender = (dotnetHelper) => {
    $(document).ready(function () {
        $('#baldType').change(function () {
            let baldType = $(this).val();
            let dynamicSection = $('#dynamic-section');
            dynamicSection.empty();
    
            if (baldType === 'veteran') {
                dynamicSection.append(`
                    <h5>Veteran/Confident Baldie</h5>
                    <div class="mb-4">
                        <label class="form-label">Do you shave or buzz?</label>
                        <input id="confidentBaldieShaveOrBuzz" type="text" class="form-control" placeholder="Shave or Buzz">
                    </div>
                    <div class="mb-4">
                        <label for="baldYears" class="form-label">How long have you been bald?</label>
                        <input type="text" class="form-control" id="baldYears" placeholder="Enter years">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Do you have the chrome cap?</label>
                        <input id="confidentBaldieChromeCap" type="text" class="form-control" placeholder="Yes or No">
                    </div>
                `);
            } else if (baldType === 'denier') {
                dynamicSection.append(`
                    <h5>The Denier Baldie</h5>
                    <div class="mb-4">
                        <label class="form-label">Are you self-conscious about being bald?</label>
                        <input id="denierBaldieSelfConscious" type="text" class="form-control" placeholder="Yes or No">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Reason?</label>
                        <input id="denierBaldieReason" type="text" class="form-control" placeholder="Enter reason">
                    </div>
                `);
            } else if (baldType === 'rookie') {
                dynamicSection.append(`
                    <h5>Rookie Up and Comer</h5>
                    <div class="mb-4">
                        <label class="form-label">Do you have a beard?</label>
                        <input id="rookieBaldieHasBeard" type="text" class="form-control" placeholder="Yes or No">
                    </div>
                    <div class="mb-4">
                        <label class="form-label">Are you looking for recommendations?</label>
                        <input id="rookieBaldieLookingForRecommendations" type="text" class="form-control" placeholder="Yes or No">
                    </div>
                `);
            } else if (baldType === 'beard_baldie') {
                dynamicSection.append(`
                    <h5>Beard Baldie</h5>
                    <div class="mb-4">
                        <label class="form-label">What type of beard do you have?</label>
                        <select class="form-select"  id="beardBaldie">
                            <option>Full Beard</option>
                            <option>Mustache</option>
                            <option>Goatee</option>
                        </select>
                    </div>
                `);
            }
        });

        $('#baldie-questionnaire').click(async function (event) {
            event.preventDefault();
        
            let answers = {
                isCompleted: true,
                data: {
                    questions: [
                        {
                            title: "How did you hear about us?",
                            answer: $('#hearAboutUs').val()
                        },
                        {
                            title: "Where do you usually buy products?",
                            answer: $('#buyProducts').val()
                        },
                        {
                            title: "Apparel size",
                            answer: $('#apparelSize').val()
                        },
                        {
                            title: "Monthly spend on grooming products",
                            answer: $('#monthlySpend').val()
                        },
                        {
                            title: "Type of bald",
                            answer: $('#baldType').val()
                        }
                    ],
                    baldingOptions: [
                        {
                            option: [
                                {
                                    baldingOptionTitle: "Beard Baldie",
                                    questions: [
                                        $(("#beardBaldie")).val()
                                    ]
                                },
                                {
                                    baldingOptionTitle: "Rookie Baldie",
                                    questions: [
                                        $('#rookieBaldieHasBeard').val(),
                                        $('#rookieBaldieLookingForRecommendations').val()
                                    ]
                                },
                                {
                                    baldingOptionTitle: "Denier Baldie",
                                    questions: [
                                        $('#denierBaldieSelfConscious').val(),
                                        $('#denierBaldieReason').val()
                                    ]
                                },
                                {
                                    baldingOptionTitle: "Confident Baldie",
                                    questions: [
                                        $('#confidentBaldieShaveOrBuzz').val(),
                                        $('#baldYears').val(),
                                        $('#confidentBaldieChromeCap').val()
                                    ]
                                }
                            ]
                        }
                    ]
                }
            };
    
            try {
                console.log('Submitting form:', answers);
                await dotnetHelper.invokeMethodAsync('SubmitQuestionnaire', JSON.stringify(answers));
                alert('Form submitted successfully!');
            } catch (error) {
                console.error('Error submitting form:', error);
            }
        });
    });
};
"use strict";




if (!window.fbControls) { window.fbControls = []; }
window.fbControls.push(function media(controlClass) {
    class currentTime extends controlClass {

        /**
         * Load embedded Javascript
         */
        configure() {
            
        }

        /**
         * Class configuration - return the icons & label related to this control
         * @return {object} definition
         */
        static get definition() {
            return {
                
                i18n: {
                    default: 'Aika',
                },
                
                
            };
        }

        /**
         * Build the HTML5 attribute for the specified media type
         * @return {Object} DOM Element to be injected into the form.
         */
        build() {

            var dtf = this.markup('input', null, { type: 'text', id: 'dateTimePicker', class: 'form-control dateTimePicker', title: 'aika', value: 'Valitse aika painamalla tästä' });
            var btn = this.markup('button', 'Ota aika', { id: 'currentTimeButton', class: 'btn-primary btn', type: 'button', style: 'default' });

            this.div = this.markup('div', [dtf, btn], );
            return this.div;

            
        }

        /**
         * onRender callback
         */
        onRender() {
          const dateTimePicker = document.getElementById('dateTimePicker');
          const currentTimeButton = document.getElementById('currentTimeButton')
            //dateTimePicker.value = new Date();
            var div = this.div;
            var picker = new Picker(dateTimePicker, {
                controls: true,
                format: 'YYYY-MM-DD HH:mm:ss',
                headers: true,
                pick: function (e) { div.value = picker.getDate()}
            });

            

            currentTimeButton.addEventListener('click', () => {

                var currentDate = new Date();
                fetch('/Aika').then(function (response) {
                    return response.json();
                }).then(function (data) {
                    console.log(data);
                    currentDate = data.aika;
                }).catch(function (err) {
                    console.log('Server Time Fetch Error :-S', err);
                });
            

              picker.setDate(currentDate);
              dateTimePicker.value = picker.getDate();
              this.div.value = picker.getDate();

          });

            

            try {
                var userDataAika = this.config.userData[0];
                

                picker.setDate(new Date(userDataAika));
                dateTimePicker.value = picker.getDate();
                this.div.value = picker.getDate();
            }
            catch (err) {
                console.log("Ei dataa currentTime elementillä");
            }
      
        }
    }

    // register this control for the following types & text subtypes
    controlClass.register('currentTime', currentTime);
    
    return currentTime;
});
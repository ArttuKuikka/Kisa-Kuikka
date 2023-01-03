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
                icon: '🕒',
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


            return this.markup('div', [dtf, btn], {data: 'PRoo data'});

            
        }

        /**
         * onRender callback
         */
        onRender() {
          const dateTimePicker = document.getElementById('dateTimePicker');
          const currentTimeButton = document.getElementById('currentTimeButton')
            //dateTimePicker.value = new Date();

            var picker = new Picker(dateTimePicker, {
                controls: true,
                format: 'YYYY-MM-DD HH:mm:ss',
                headers: true,
                
            });

            

          currentTimeButton.addEventListener('click', () =>{
            const currentDate = new Date();

              dateTimePicker.value = currentDate;
              
              

          });

            
      
        }
    }

    // register this control for the following types & text subtypes
    controlClass.register('currentTime', currentTime);
    
    return currentTime;
});
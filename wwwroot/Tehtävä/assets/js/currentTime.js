"use strict";
/**
 * This file is part of the Media (Image,Video,Audio) Element for formBuilder.
 * https://github.com/lucasnetau/formBuilder-plugin-media
 *
 * (c) James Lucas <james@lucas.net.au>
 *
 * @license MIT
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 */
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

            var dtf = this.markup('input', null, { type: 'datetime-local', id: 'dateTimePicker', class: 'dateTimePicker', title: 'aika', value: 'date-value' });
            var btn = this.markup('button', 'Ota aika', { id: 'currentTimeButton', class: 'btn-primary btn', type: 'button', style: 'default' });


            return this.markup('div', [dtf, btn], {value: 'div-value'});

            
        }

        /**
         * onRender callback
         */
        onRender() {
          const dateTimePicker = document.getElementById('dateTimePicker');
          const currentTimeButton = document.getElementById('currentTimeButton')
          
          var value = this.config.value || 0;
          dateTimePicker.value = value;

          currentTimeButton.addEventListener('click', () =>{
            const currentDate = new Date();

            dateTimePicker.value = currentDate.toISOString().slice(0, -1);
              this.config.value = currentDate.toISOString().slice(0, -1);
              

          });
      
        }
    }

    // register this control for the following types & text subtypes
    controlClass.register('currentTime', currentTime);
    
    return currentTime;
});
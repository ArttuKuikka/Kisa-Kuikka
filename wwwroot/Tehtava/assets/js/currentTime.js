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
                defaultAttrs: {
                    'tarkista': {
                        label: 'Tarkista',
                        value: true,
                        type: 'checkbox'
                    },
                    'lataa': {
                        label: 'Sisällytä lataukseen',
                        value: true,
                        type: 'checkbox'
                    }
                }
                
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
                format: 'DD.MM.YYYY HH:mm:ss',
                headers: true,
                pick: function (e) { var aika = picker.getDate(); aika.setMilliseconds(0); div.value = aika.toISOString(); console.log(div.value); }
            });

            

            currentTimeButton.addEventListener('click', () => {

                let tama = this;
                let currentDate;
                fetch('/Aika').then(function (response) {
                    return response.json();
                }).then(function (data) {
                    console.log(data);
                    currentDate = data.aika;

                    if (typeof currentDate === 'undefined') {
                        alert("Virhe asettaessa aikaa, syötä aika manuaalisesti");
                        return;
                    }

                    picker.setDate(currentDate);
                    var dtoptions = { day: '2-digit', year: 'numeric', month: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' }
                    dateTimePicker.value = picker.getDate().toLocaleDateString('fi-FI', dtoptions);
                    var aika = picker.getDate();
                    aika.setMilliseconds(0);
                    tama.div.value = aika.toISOString();

                    console.log(tama.div.value);

                }).catch(function (err) {
                    console.log('virhe asettaessa aikaa', err);
                });

                
          });

            

            try {
                var userDataAika = this.config.userData[0];
                

                picker.setDate(new Date(userDataAika));
                var dtoptions = { day: '2-digit', year: 'numeric', month: '2-digit', hour: '2-digit', minute: '2-digit', second: '2-digit' }
                dateTimePicker.value = picker.getDate().toLocaleDateString('fi-FI', dtoptions);
                var aika = picker.getDate();
                aika.setMilliseconds(0);
                this.div.value = aika.toISOString();
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
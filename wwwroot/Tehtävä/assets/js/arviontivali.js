"use strict";




if (!window.fbControls) { window.fbControls = []; }
window.fbControls.push(function media(controlClass) {
    class arviontivali extends controlClass {

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
                icon: '5️⃣',
                i18n: {
                    default: 'Arviontiväli',
                },
                defaultAttrs: {
                    'min': {
                        label: 'Min',
                        value: 1,
                        type: 'number'
                    },
                    'max': {
                        label: 'Max',
                        value: 5,
                        type: 'number'
                    },
                    }
                
                
            };
        }

        /**
         * Build the HTML5 attribute for the specified media type
         * @return {Object} DOM Element to be injected into the form.
         */
        build() {
            const { ...attrs } = this.config;
            var btnarray = [];
            for (let i = attrs.min; i < attrs.max + 1; i++) {
                var btn = this.markup('button', i.toString(), {id: 'arviontivalibtn-' + i.toString(), class: 'btn-primary btn', type: 'button', style: 'default' });
                btnarray.push(btn);
            };
            this.btnarray = btnarray;
            return this.markup('div', btnarray, );

        }

        /**
         * onRender callback
         */
        onRender() {
            console.log(this.btnarray);
            for (let i = 0; i < this.btnarray.length; i++) {
                var btn = this.btnarray[i];
                console.log(btn);
                btn.addEventListener('click', () => {
                    console.log('click on ' + (i + 1).toString()); //tähän jotai siit mitä tulee userdataks
                });
            }
           
            
      
        }
    }

    // register this control for the following types & text subtypes
    controlClass.register('arviontivali', arviontivali);
    
    return arviontivali;
});
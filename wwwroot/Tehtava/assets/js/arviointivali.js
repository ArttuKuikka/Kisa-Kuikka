"use strict";




if (!window.fbControls) { window.fbControls = []; }
window.fbControls.push(function media(controlClass) {
    class arviointivali extends controlClass {

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
                    default: 'Arviointiväli',
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
                var btn = this.markup('button', i.toString(), {id: 'arviontivalibtn-' + i.toString(), class: 'btn-primary btn', type: 'button', style: 'default', value: 88 });
                btnarray.push(btn);
            };
            this.btnarray = btnarray;
            this.div = this.markup('div', btnarray,);
            return this.div;

        }

        /**
         * onRender callback
         */
        onRender() {
            
            for (let i = 0; i < this.btnarray.length; i++) {
                var btn = this.btnarray[i];
                
                btn.addEventListener('click', () => {
                    //console.log('click on ' + (i + 1).toString());
                     
                     this.div.value = (i + 1).toString();
                });
            }
           
            
      
        }
    }

    // register this control for the following types & text subtypes
    controlClass.register('arviointivali', arviointivali);
    
    return arviointivali;
});
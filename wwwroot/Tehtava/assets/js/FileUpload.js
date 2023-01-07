"use strict";




if (!window.fbControls) { window.fbControls = []; }
window.fbControls.push(function media(controlClass) {
    class fileUpload extends controlClass {

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
                icon: '⬆️',
                i18n: {
                    default: 'Tiedoston lataus',
                },


            };
        }

        /**
         * Build the HTML5 attribute for the specified media type
         * @return {Object} DOM Element to be injected into the form.
         */
        build() {

            this.fileinput = this.markup('input', null, { type: 'file', class: 'form-control ', title: 'Lataa tiedosto' });
            this.form = this.markup('form', this.fileinput, { method: 'post', enctype: 'multipart/form-data', action:'/Tiedosto/Post'})
            this.statustext = this.markup('p', 'odotetaan tiedostoa')
            this.div = this.markup('div', [this.form, this.statustext]);
            return this.div;
            

        }

        /**
         * onRender callback
         */
        onRender() {
            this.fileinput.addEventListener('change', () => {
                try {
                    this.statustext.innerHTML = 'Tiedostoa ladataan palvelimelle'
                    
                    let formData = new FormData();
                    var file = this.fileinput.files[0];
                    formData.append('File', file)
                    fetch('/Tiedosto/Post', {
                        method: 'POST',


                        body: formData
                    })
                        .then(response => response.json())
                        .then(response => this.div.value = JSON.stringify(response))


                   
                    this.statustext.innerHTML = 'Ladattu'
                }
                catch (error) {
                    this.statustext.innerHTML = 'Virhe ladatessa tiedostoa'
                }
            });
            



            



        }
    }

    // register this control for the following types & text subtypes
    controlClass.register('fileUpload', fileUpload);

    return fileUpload;
});
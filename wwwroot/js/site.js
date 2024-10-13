// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Funcion para editar Perfil de Usuario

document.addEventListener('DOMContentLoaded', function () {
    // Evento para abrir el selector de archivos al hacer clic en la imagen del perfil
    const profilePicInput = document.getElementById('profile-pic-input');

    // Maneja el cambio en el input de archivo
    profilePicInput.addEventListener('change', function (event) {
        const fileInput = event.target;
        if (fileInput.files.length > 0) {
            const form = fileInput.closest('form');
            form.submit(); // Enviar el formulario automáticamente
        }
    });

    // Abre el selector de archivos al hacer clic en la imagen de perfil
    const profilePicLabel = document.querySelector('label[for="profile-pic-input"]');
    profilePicLabel.addEventListener('click', function () {
        profilePicInput.click();
    });
});

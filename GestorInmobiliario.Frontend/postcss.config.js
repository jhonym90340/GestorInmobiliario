    // postcss.config.js
    // Configuración para PostCSS, utilizado por Tailwind CSS
    export default {
      plugins: {
        // Usa el plugin de anidación de PostCSS de Tailwind
        '@tailwindcss/nesting': {},
        // ¡El cambio clave: ahora se usa el paquete específico de PostCSS para Tailwind!
        '@tailwindcss/postcss': {},
        // Plugin para añadir prefijos de navegador automáticamente
        autoprefixer: {},
      },
    }
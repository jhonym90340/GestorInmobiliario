// tailwind.config.js

/** @type {import('tailwindcss').Config} */
module.exports = {
  // Configura los archivos que Tailwind debe escanear para generar las clases CSS
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  // Define y extiende el tema predeterminado de Tailwind
  theme: {
    extend: {
      // Personaliza la paleta de colores con nombres significativos
      colors: {
        'primary-indigo': '#4F46E5',       // Un índigo vibrante para elementos principales
        'secondary-purple': '#8B5CF6',     // Un púrpura para acentos y elementos secundarios
        'light-gray-bg': '#F9FAFB',        // Un gris muy claro para fondos generales
        'card-gradient-start': '#FFFFFF',  // Blanco puro para el inicio de gradientes en tarjetas
        'card-gradient-end': '#F9FAFB',    // Un gris muy claro para el final de gradientes en tarjetas
        'text-dark-gray': '#374151',       // Gris oscuro para textos principales
        'text-light-gray': '#6B7280',      // Gris más claro para textos secundarios o descripciones
      },
      // Define familias de fuentes personalizadas
      fontFamily: {
        inter: ['Inter', 'sans-serif'],    // Asegura que 'Inter' sea una opción de fuente
      },
      // Crea sombras personalizadas para dar profundidad a los elementos
      boxShadow: {
        'soft-xl': '0 20px 25px -5px rgba(0, 0, 0, 0.08), 0 10px 10px -5px rgba(0, 0, 0, 0.05)', // Sombra grande y suave
        'custom-lg': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)', // Sombra estándar más pronunciada
        'button-glow': '0 0px 15px rgba(139, 92, 246, 0.6)', // Una sombra tipo 'glow' para botones
      },
      // Puedes extender otras propiedades como el espaciado, los tamaños, etc.
      spacing: {
        '18': '4.5rem', // Ejemplo de espaciado adicional
      }
    },
  },
  // Plugins de Tailwind CSS (generalmente no se necesitan para diseño básico)
  plugins: [],
}

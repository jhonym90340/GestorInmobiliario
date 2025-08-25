// src/index.js (o src/main.jsx si usas Vite)

import React from 'react'; // Importa la librería React
import ReactDOM from 'react-dom/client'; // Importa ReactDOM para el renderizado del DOM
import './index.css'; // Importa tu archivo CSS global (donde Tailwind CSS está configurado)
import App from './App'; // Importa el componente principal de tu aplicación (App.jsx)

// Obtiene el elemento DOM donde se montará la aplicación React
const rootElement = document.getElementById('root');

// Crea una raíz de React para tu aplicación.
// Esto es parte de la API de React 18 para un renderizado concurrente y más eficiente.
const root = ReactDOM.createRoot(rootElement);

// Renderiza el componente principal de tu aplicación dentro del modo estricto de React.
// React.StrictMode es una herramienta para destacar problemas potenciales en la aplicación.
// No renderiza ninguna UI visible, solo activa advertencias y comprobaciones adicionales.
root.render(
  <React.StrictMode>
    <App /> {/* Renderiza el componente principal 'App' */}
  </React.StrictMode>
);
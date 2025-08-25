import React, { forwardRef } from 'react'; // <-- IMPORTAR forwardRef

// Botón reutilizable
export const Button = ({ children, className = '', ...props }) => (
    <button
        className={`
            px-6 py-3 md:px-8 md:py-4
            bg-indigo-700 text-white font-semibold rounded-lg
            shadow-lg hover:shadow-xl hover:bg-indigo-800
            focus:outline-none focus:ring-4 focus:ring-indigo-300
            transition duration-300 ease-in-out transform hover:-translate-y-1 active:scale-95
            ${className}
        `}
        {...props}
    >
        {children}
    </button>
);

// Input reutilizable

export const Input = forwardRef(({ className = '', ...props }, ref) => (
    <input
        ref={ref} 
        className={`
            w-full p-3 md:p-4 border border-gray-300 rounded-lg
            focus:ring-2 focus:ring-indigo-500 focus:border-transparent
            transition duration-200 outline-none
            text-gray-800 placeholder-gray-500
            ${className}
        `}
        {...props}
    />
));

// Select (dropdown) reutilizable
export const Select = ({ children, className = '', ...props }) => (
    <select
        className={`
            w-full p-3 md:p-4 border border-gray-300 rounded-lg
            focus:ring-2 focus:ring-indigo-500 focus:border-transparent
            transition duration-200 outline-none
            text-gray-800
            ${className}
        `}
        {...props}
    >
        {children}
    </select>
);

// Tarjeta (Card) reutilizable
export const Card = ({ children, className = '', ...props }) => (
    <div
        className={`
            bg-white p-6 rounded-lg shadow-md
            ${className}
        `}
        {...props}
    >
        {children}
    </div>
);
import React, { useState } from 'react';

const ImageWithFallback = ({ 
    src, 
    alt, 
    className, 
    fallbackSrc = "https://placehold.co/300x200/F0F0F0/333333?text=Imagen+No+Disponible" 
}) => {
    const [imageError, setImageError] = useState(false);

    const handleError = () => {
        setImageError(true);
    };

    const getImageUrl = (url) => {
        if (!url) return fallbackSrc;
        
        // Si ya es una URL completa
        if (url.startsWith('http')) return url;
        
        // Si es una ruta relativa
        if (url.startsWith('/')) {
            return `http://localhost:50000${url}`;
        }
        
        // Para cualquier otro caso
        return `http://localhost:50000/images/${url}`;
    };

    return (
        <img
            src={imageError ? fallbackSrc : getImageUrl(src)}
            alt={alt}
            className={className}
            onError={handleError}
        />
    );
};

export default ImageWithFallback;
import React, { useState, useEffect } from 'react';
import { Button } from './common.jsx';
import PropTypes from 'prop-types';
import axios from 'axios';
import ImageWithFallback from './ImageWithFallback'; // Importar el componente

const PropertyImageManager = ({ property, onClose, apiUrl, apiUrlImages }) => {
    const [selectedFiles, setSelectedFiles] = useState([]);
    const [propertyImages, setPropertyImages] = useState([]);
    const [uploading, setUploading] = useState(false);

    // Cargar imágenes existentes al abrir el modal
    useEffect(() => {
        fetchPropertyImages();
    }, [property]);

    const fetchPropertyImages = async () => {
        try {
            const response = await axios.get(`${apiUrl}/${property.idProperty}/images`);
            setPropertyImages(response.data);
        } catch (error) {
            console.error('Error al cargar imágenes:', error);
        }
    };

    const handleImageUpload = async (event) => {
        const files = event.target.files;
        if (!files || files.length === 0) return;

        setUploading(true);
        try {
            const formData = new FormData();
            formData.append('imageFile', files[0]);

            console.log('Subiendo imagen:', files[0].name);
            console.log('Tamaño:', files[0].size, 'bytes');
            console.log('Tipo:', files[0].type);

            const response = await axios.post(
                `${apiUrl}/${property.idProperty}/upload-image`,
                formData,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    },
                    timeout: 30000 // 30 segundos timeout
                }
            );

            console.log('Imagen subida exitosamente:', response.data);

            // Actualizar la lista de imágenes
            await fetchPropertyImages();
            setSelectedFiles([]);

        } catch (error) {
            console.error('Error completo al subir imagen:', error);

            // Mostrar mensaje de error más detallado
            if (error.response) {
                // El servidor respondió con un código de error
                console.error('Respuesta del servidor:', error.response.data);
                console.error('Status:', error.response.status);
                alert(`Error del servidor: ${error.response.status} - ${JSON.stringify(error.response.data)}`);
            } else if (error.request) {
                // La petición fue hecha pero no se recibió respuesta
                console.error('No se recibió respuesta del servidor');
                alert('No se pudo conectar con el servidor. Verifica que el backend esté ejecutándose.');
            } else {
                // Error al configurar la petición
                console.error('Error configurando la petición:', error.message);
                alert(`Error: ${error.message}`);
            }
        } finally {
            setUploading(false);
        }
    };
    
    const handleDeleteImage = async (imageUrl) => {
        if (!window.confirm('¿Estás seguro de que quieres eliminar esta imagen?')) {
            return;
        }

        try {
            await axios.delete(`${apiUrl}/delete-image`, {
                data: {
                    propertyId: property.idProperty,
                    imageUrl: imageUrl
                }
            });

            // Actualizar la lista de imágenes
            await fetchPropertyImages();
        } catch (error) {
            console.error('Error al eliminar imagen:', error);
            alert('Error al eliminar la imagen');
        }
    };

    return (
        <div className="p-6">
            <h3 className="text-2xl font-semibold mb-6 text-gray-800 text-center">
                Gestión de Imágenes para: {property.name}
            </h3>

            {/* Sección de subida de imágenes */}
            <div className="mb-8 p-4 bg-gray-50 rounded-lg">
                <h4 className="text-lg font-medium mb-4 text-gray-700">Subir Nueva Imagen</h4>
                <input
                    type="file"
                    accept="image/*"
                    onChange={handleImageUpload}
                    disabled={uploading}
                    className="w-full p-3 border border-gray-300 rounded-lg mb-3"
                />
                {uploading && (
                    <p className="text-blue-600 text-sm">Subiendo imagen...</p>
                )}
            </div>

            {/* Galería de imágenes existentes */}
            <div className="mb-6">
                <h4 className="text-lg font-medium mb-4 text-gray-700">Imágenes Existentes</h4>

                {propertyImages.length === 0 ? (
                    <p className="text-gray-500 text-center py-8">No hay imágenes para esta propiedad</p>
                ) : (
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {propertyImages.map((imageUrl, index) => (
                            <div key={index} className="relative group">
                                {/*  */}
                                <ImageWithFallback
                                    src={imageUrl}
                                    alt={`Imagen ${index + 1} de ${property.name}`}
                                    className="w-full h-48 object-cover rounded-lg shadow-md"
                                    fallbackSrc="https://placehold.co/300x200/F0F0F0/333333?text=Imagen+No+Disponible"
                                />
                                <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-50 transition-all duration-300 rounded-lg flex items-center justify-center opacity-0 group-hover:opacity-100">
                                    <Button
                                        onClick={() => handleDeleteImage(imageUrl)}
                                        className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded"
                                    >
                                        Eliminar
                                    </Button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {/* Botones de acción */}
            <div className="flex justify-end space-x-4">
                <Button
                    onClick={onClose}
                    className="bg-gray-500 hover:bg-gray-600"
                >
                    Cerrar
                </Button>
            </div>
        </div>
    );
};

PropertyImageManager.propTypes = {
    property: PropTypes.object.isRequired,
    onClose: PropTypes.func.isRequired,
    apiUrl: PropTypes.string.isRequired,
    apiUrlImages: PropTypes.string.isRequired,
};

export default PropertyImageManager;
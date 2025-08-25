import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Card } from '../components/common.jsx';
import ImageWithFallback from '../components/ImageWithFallback';
import { useNavigate } from 'react-router-dom'; // Importar useNavigate

const Home = () => {
    const [properties, setProperties] = useState([]);
    const navigate = useNavigate(); // Hook para navegación

    // URL de tu API para obtener propiedades
    const API_URL_PROPERTIES = 'http://localhost:50000/api/properties';

    // useEffect para cargar las propiedades cuando el componente se monta
    useEffect(() => {
        const fetchPropertiesForHome = async () => {
            try {
                const response = await axios.get(API_URL_PROPERTIES);
                const recentProperties = response.data.slice(0, 6);
                setProperties(recentProperties);
                console.log('Propiedades cargadas para Home:', recentProperties);
            } catch (error) {
                console.error('Error al cargar propiedades para Home:', error);
            }
        };

        fetchPropertiesForHome();
    }, []);

    // Función para ver detalles de la propiedad
    const handleViewDetails = (propertyId) => {
        console.log('Ver detalles de propiedad:', propertyId);
        // Navegar a la página de gestión de propiedades con el ID específico
        navigate('/properties', { 
            state: { 
                viewPropertyDetails: true, 
                propertyId: propertyId 
            } 
        });
    };

    // Función para crear un modal de detalles (alternativa)
    const handleShowDetailsModal = (property) => {
        console.log('Mostrar modal de detalles:', property);
       
        alert(`Detalles de: ${property.name}\nDirección: ${property.address}\nPrecio: $${property.price}\nCódigo: ${property.codeInternal}`);
    };

    const PropertyList = ({ propertiesToList, title }) => (
        <div className="py-8 md:py-12">
            <h3 className="text-3xl md:text-4xl font-bold text-center mb-10 text-gray-800">
                {title}
            </h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
                {propertiesToList.length > 0 ? (
                    propertiesToList.map((property) => (
                        <Card
                            key={property.idProperty || property.id || property._id}
                            className="group transform hover:scale-105 hover:shadow-xl transition duration-300 ease-in-out flex flex-col cursor-pointer"
                        >
                            <div className="relative w-full h-48 flex-shrink-0 overflow-hidden rounded-t-xl flex justify-center items-center bg-gray-100">
                                {property.imageUrls && property.imageUrls.length > 0 ? (
                                    <ImageWithFallback
                                        src={property.imageUrls[0]}
                                        alt={property.name}
                                        className="w-full h-48 object-cover transition-transform duration-300 group-hover:scale-110"
                                        fallbackSrc="https://placehold.co/400x300/F0F0F0/333333?text=Imagen+No+Disponible"
                                    />
                                ) : (
                                    <div className="flex items-center justify-center w-full h-48 bg-gray-200">
                                        <span className="text-gray-500 text-lg">Sin Imagen</span>
                                    </div>
                                )}
                                <div className="absolute inset-0 bg-gradient-to-t from-black via-transparent to-transparent opacity-30"></div>
                                
                                {/* Badge de precio */}
                                <div className="absolute top-4 right-4 bg-green-500 text-white px-3 py-1 rounded-full text-sm font-semibold">
                                    €{property.price ? parseFloat(property.price).toLocaleString('es-ES') : 'N/A'}
                                </div>
                            </div>

                            <div className="p-6 flex flex-col flex-grow">
                                <h3 className="text-xl md:text-2xl font-semibold mb-2 text-gray-800 line-clamp-1">
                                    {property.name}
                                </h3>
                                <p className="text-gray-600 mb-2 text-sm md:text-base line-clamp-2">
                                    📍 {property.address}
                                </p>
                                
                                <div className="grid grid-cols-2 gap-2 mb-4">
                                    <p className="text-gray-600 text-sm">
                                        <span className="font-semibold">Código:</span><br/>
                                        {property.codeInternal}
                                    </p>
                                    <p className="text-gray-600 text-sm">
                                        <span className="font-semibold">Año:</span><br/>
                                        {property.year}
                                    </p>
                                </div>
                                
                                <Button 
                                    onClick={() => handleViewDetails(property.idProperty)}
                                    className="w-full mt-auto bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded-lg shadow-md transition duration-300"
                                >
                                    <div className="flex items-center justify-center">
                                        <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"></path>
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"></path>
                                        </svg>
                                        Ver Detalles
                                    </div>
                                </Button>

                                {/* Botón alternativo para modal */}
                                <Button 
                                    onClick={() => handleShowDetailsModal(property)}
                                    className="w-full mt-2 bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded-lg shadow-md transition duration-300"
                                >
                                    <div className="flex items-center justify-center">
                                        <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
                                        </svg>
                                        Info Rápida
                                    </div>
                                </Button>
                            </div>
                        </Card>
                    ))
                ) : (
                    <div className="col-span-full text-center py-12">
                        <p className="text-gray-500 text-lg mb-4">No hay propiedades disponibles.</p>
                        <Button 
                            onClick={() => navigate('/properties')}
                            className="bg-indigo-600 hover:bg-indigo-700"
                        >
                            Crear Primera Propiedad
                        </Button>
                    </div>
                )}
            </div>
        </div>
    );

    return (
        <>
            {/* Sección de Bienvenida/Hero */}
            <section className="relative h-96 bg-cover bg-center flex flex-col justify-end text-white overflow-hidden py-16 px-4">
                <img
                    src="https://images.unsplash.com/photo-1560518883-ce09059eeffa?ixlib=rb-4.0.3&auto=format&fit=crop&w=1920&h=1080&q=80"
                    alt="Fondo de Panel Administrativo de Propiedades"
                    className="absolute inset-0 w-full h-full object-cover z-0 filter brightness-75"
                    onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/1920x1080/0A0A0A/0A0A0A"; }}
                />
                <div className="relative z-10 text-center max-w-4xl mx-auto w-full mb-8 md:mb-16">
                    <h1 className="text-4xl md:text-6xl font-extrabold leading-tight drop-shadow-lg mb-4 md:mb-6">
                        Gestión Integral de tu Cartera Inmobiliaria
                    </h1>
                    <p className="text-lg md:text-xl font-light mb-8 md:mb-12 drop-shadow-md">
                        Accede, organiza y administra tus propiedades y sus trazabilidades de forma eficiente.
                    </p>
                </div>
            </section>

            {/* Sección de Resumen de Propiedades Recientes */}
            <section className="py-16 md:py-20 px-4 md:px-12 max-w-7xl mx-auto">
                <PropertyList propertiesToList={properties} title="Nuestras Últimas Propiedades" />
                
                {properties.length > 0 && (
                    <div className="text-center mt-12">
                        <Button
                            onClick={() => navigate('/properties')}
                            className="bg-indigo-600 hover:bg-indigo-700 text-white font-bold py-3 px-8 rounded-lg shadow-lg transition duration-300 text-lg"
                        >
                            <div className="flex items-center justify-center">
                                <svg className="w-6 h-6 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-2m2 0V9m0 0H5m14 0V3"></path>
                                </svg>
                                Ver Todas las Propiedades
                            </div>
                        </Button>
                    </div>
                )}
            </section>

           
        </>
    );
};

export default Home;
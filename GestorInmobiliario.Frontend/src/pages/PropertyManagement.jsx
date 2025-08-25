import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Input, Select, Card } from '../components/common.jsx';
import PropertyImageManager from '../components/PropertyImageManager';

const PropertyManagement = () => {
    // --- Estados del Componente ---
    const [properties, setProperties] = useState([]);
    const [owners, setOwners] = useState([]);
    const [newProperty, setNewProperty] = useState({
        name: '',
        address: '',
        price: '',
        codeInternal: '',
        year: '',
        idOwner: '',
    });
    const [newImageFile, setNewImageFile] = useState(null);

    const [filters, setFilters] = useState({
        name: '',
        address: '',
        priceMin: '',
        priceMax: ''
    });

    // Estados para la funcionalidad de edición
    const [editingProperty, setEditingProperty] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);
    const [editImageFile, setEditImageFile] = useState(null);

    // Estados para la funcionalidad de eliminación
    const [propertyToDelete, setPropertyToDelete] = useState(null);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

    // Estados para la gestión de imágenes
    const [selectedPropertyForImages, setSelectedPropertyForImages] = useState(null);
    const [showImageManager, setShowImageManager] = useState(false);

    // URLs de las APIs
    const API_URL_PROPERTIES = 'http://localhost:50000/api/properties';
    const API_URL_OWNERS = 'http://localhost:50000/api/owners';
    const API_URL_UPLOAD_IMAGE = 'http://localhost:50000/api/properties/upload-image';
    const API_URL_IMAGES = 'http://localhost:50000/images';

    // --- Funciones de Fetch ---

    const fetchProperties = async () => {
        try {
            console.log('Buscando propiedades...');
            const params = new URLSearchParams(filters);

            const uniqueParam = new Date().getTime();
            params.append('_', uniqueParam);

            const response = await axios.get(`${API_URL_PROPERTIES}?${params}`);
            console.log("Propiedades obtenidas:", response.data);

            const formattedProperties = response.data.map(prop => ({
                idProperty: prop.idProperty,
                name: prop.name,
                address: prop.address,
                price: prop.price,
                codeInternal: prop.codeInternal,
                year: prop.year,
                idOwner: prop.idOwner,
                imageUrls: prop.imageUrls || [],
                owner: owners.find(o => o.idOwner === prop.idOwner) || null,
            }));

            setProperties(formattedProperties);

        } catch (error) {
            console.error('Error al obtener las propiedades:', error);
        }
    };

    const fetchOwners = async () => {
        try {
            const response = await axios.get(API_URL_OWNERS);
            setOwners(response.data);
            console.log("Propietarios obtenidos:", response.data);
        } catch (error) {
            console.error('Error al obtener los propietarios:', error);
        }
    };

    // --- Hooks de Efecto ---
    useEffect(() => {
        fetchOwners();
    }, []);

    useEffect(() => {
        if (owners.length > 0) {
            fetchProperties();
        }
    }, [filters, owners]);

    // --- Handlers de Eventos ---
    const handleNewPropertyChange = (e) => {
        const { name, value } = e.target;
        setNewProperty(prevState => ({ ...prevState, [name]: value }));
    };

    const handleNewImageChange = (e) => {
        setNewImageFile(e.target.files[0]);
    };

    const handleFiltersChange = (e) => {
        const { name, value } = e.target;
        setFilters(prevState => ({ ...prevState, [name]: value }));
    };



    const handleAddPropertySubmit = async (e) => {
        e.preventDefault();

        try {
            const propertyToCreate = {
                idOwner: newProperty.idOwner,
                name: newProperty.name,
                address: newProperty.address,
                price: parseFloat(newProperty.price),
                codeInternal: newProperty.codeInternal,
                year: parseInt(newProperty.year)
            };

            const addResponse = await axios.post(API_URL_PROPERTIES, propertyToCreate);
            const newPropertyId = addResponse.data.idProperty;

            if (newImageFile && newPropertyId) {
                uploadImageInBackground(newPropertyId, newImageFile);
            }

            setNewProperty({ name: '', address: '', price: '', codeInternal: '', year: '', idOwner: '' });
            setNewImageFile(null);

            await fetchProperties();

            alert('✅ Propiedad creada exitosamente');

        } catch (error) {
            console.error('Error al crear propiedad:', error);
            console.log('Respuesta completa del error:', error.response);

            // MANEJO MEJORADO DE ERRORES DE VALIDACIÓN
            if (error.response?.data?.errors) {
                // El backend devolvió errores de validación
                const validationErrors = error.response.data.errors;
                let errorMessage = '❌ Por favor corrige los siguientes errores:\n\n';

                // Mapear los errores a mensajes amigables
                Object.keys(validationErrors).forEach(field => {
                    const fieldErrors = validationErrors[field];
                    fieldErrors.forEach(errorMsg => {
                        // Traducir nombres de campos y mensajes
                        let fieldName = field;
                        let friendlyMessage = errorMsg;

                        // Traducir nombres de campos
                        switch (field) {
                            case 'Name':
                                fieldName = 'Nombre';
                                break;
                            case 'Address':
                                fieldName = 'Dirección';
                                break;
                            case 'Price':
                                fieldName = 'Precio';
                                break;
                            case 'CodeInternal':
                                fieldName = 'Código Interno';
                                break;
                            case 'Year':
                                fieldName = 'Año';
                                break;
                            case 'IdOwner':
                                fieldName = 'Propietario';
                                break;
                        }

                        // Traducir mensajes comunes de validación
                        if (errorMsg.includes('required') || errorMsg.includes('obligatorio')) {
                            friendlyMessage = 'es obligatorio';
                        } else if (errorMsg.includes('minimum length') || errorMsg.includes('mínimo')) {
                            friendlyMessage = 'es demasiado corto';
                        } else if (errorMsg.includes('greater than') || errorMsg.includes('mayor')) {
                            friendlyMessage = 'debe ser mayor';
                        }

                        errorMessage += `• ${fieldName}: ${friendlyMessage}\n`;
                    });
                });

                alert(errorMessage);
            }
            else if (error.response?.data?.CodeInternal) {
                alert('❌ CÓDIGO DUPLICADO\n\nEl código interno "' + newProperty.codeInternal + '" ya existe.\n\nPor favor elija otro código diferente.');
            }
            else if (error.response?.data) {
                const responseData = error.response.data;
                const errorString = JSON.stringify(responseData).toLowerCase();

                if (errorString.includes('código interno') || errorString.includes('codeinternal') ||
                    errorString.includes('duplicado') || errorString.includes('ya existe')) {
                    alert('❌ CÓDIGO DUPLICADO\n\nEl código interno "' + newProperty.codeInternal + '" ya existe.\n\nPor favor elija otro código diferente.');
                }
                else if (responseData.title) {
                    alert('❌ Error: ' + responseData.title);
                }
                else if (typeof responseData === 'string') {
                    if (responseData.toLowerCase().includes('código interno') ||
                        responseData.toLowerCase().includes('codeinternal') ||
                        responseData.toLowerCase().includes('duplicado')) {
                        alert('❌ CÓDIGO DUPLICADO\n\nEl código interno "' + newProperty.codeInternal + '" ya existe.\n\nPor favor elija otro código diferente.');
                    } else {
                        alert('❌ Error: ' + responseData);
                    }
                }
                else {
                    alert('❌ Error al crear la propiedad. Por favor intente nuevamente.');
                }
            }
            else {
                alert('❌ Error al crear la propiedad. Por favor intente nuevamente.');
            }
        }
    };




    const uploadImageInBackground = async (propertyId, imageFile) => {
        try {
            const formData = new FormData();
            formData.append('imageFile', imageFile);

            await axios.post(
                `${API_URL_PROPERTIES}/${propertyId}/upload-image`,
                formData,
                {
                    headers: { 'Content-Type': 'multipart/form-data' },
                    timeout: 10000
                }
            );

            console.log(' Imagen subida exitosamente en background');
            setTimeout(() => fetchProperties(), 1000);

        } catch (imageError) {
            console.warn('⚠️ Error subiendo imagen (no crítico):', imageError.response?.data || imageError.message);
        }
    };

    const uploadPropertyImage = async (propertyId, imageFile) => {
        try {
            const formData = new FormData();
            formData.append('imageFile', imageFile);

            const config = {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
                timeout: 15000,
                onUploadProgress: (progressEvent) => {
                    const percentCompleted = Math.round(
                        (progressEvent.loaded * 100) / progressEvent.total
                    );
                    console.log(`Subiendo imagen: ${percentCompleted}%`);
                }
            };

            const response = await axios.post(
                `${API_URL_PROPERTIES}/${propertyId}/upload-image`,
                formData,
                config
            );

            return response.data;

        } catch (error) {
            if (error.code === 'ECONNABORTED') {
                console.warn('⚠️ Timeout subiendo imagen - muy grande o conexión lenta');
            } else if (error.response?.status === 413) {
                console.warn('⚠️ Imagen demasiado grande');
            } else {
                console.warn('⚠️ Error subiendo imagen:', error.response?.data || error.message);
            }
            throw error;
        }
    };

    const handleEditClick = (property) => {
        setEditingProperty({ ...property });
        setShowEditModal(true);
    };

    const handleEditChange = (e) => {
        const { name, value } = e.target;
        setEditingProperty(prevState => ({ ...prevState, [name]: value }));
    };

    const handleEditImageChange = (e) => {
        setEditImageFile(e.target.files[0]);
    };




    const handleEditSubmit = async (e) => {
    e.preventDefault();
    try {
        const updatedProperty = {
            idProperty: editingProperty.idProperty,
            idOwner: editingProperty.idOwner,
            name: editingProperty.name,
            address: editingProperty.address,
            price: parseFloat(editingProperty.price),
            codeInternal: editingProperty.codeInternal,
            year: parseInt(editingProperty.year)
        };
        
        await axios.put(`${API_URL_PROPERTIES}/${editingProperty.idProperty}`, updatedProperty);

        if (editImageFile) {
            const formData = new FormData();
            formData.append('imageFile', editImageFile);
            formData.append('propertyId', editingProperty.idProperty);
            await axios.post(API_URL_UPLOAD_IMAGE, formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });
        }

        setShowEditModal(false);
        setEditingProperty(null);
        setEditImageFile(null);
        fetchProperties();
        
        alert('✅ Propiedad actualizada exitosamente');
        
    } catch (error) {
        console.error('Error al editar la propiedad:', error);
        console.log('Respuesta completa del error:', error.response);
        
        // MANEJO MEJORADO DE ERRORES DE VALIDACIÓN
        if (error.response?.data?.errors) {
            const validationErrors = error.response.data.errors;
            let errorMessage = '❌ Por favor corrige los siguientes errores:\n\n';
            
            Object.keys(validationErrors).forEach(field => {
                const fieldErrors = validationErrors[field];
                fieldErrors.forEach(errorMsg => {
                    let fieldName = field;
                    let friendlyMessage = errorMsg;
                    
                    switch (field) {
                        case 'Name':
                            fieldName = 'Nombre';
                            break;
                        case 'Address':
                            fieldName = 'Dirección';
                            break;
                        case 'Price':
                            fieldName = 'Precio';
                            break;
                        case 'CodeInternal':
                            fieldName = 'Código Interno';
                            break;
                        case 'Year':
                            fieldName = 'Año';
                            break;
                        case 'IdOwner':
                            fieldName = 'Propietario';
                            break;
                    }
                    
                    if (errorMsg.includes('required') || errorMsg.includes('obligatorio')) {
                        friendlyMessage = 'es obligatorio';
                    } else if (errorMsg.includes('minimum length') || errorMsg.includes('mínimo')) {
                        friendlyMessage = 'es demasiado corto';
                    } else if (errorMsg.includes('greater than') || errorMsg.includes('mayor')) {
                        friendlyMessage = 'debe ser mayor';
                    }
                    
                    errorMessage += `• ${fieldName}: ${friendlyMessage}\n`;
                });
            });
            
            alert(errorMessage);
        }
        else if (error.response?.data?.CodeInternal) {
            alert('❌ CÓDIGO DUPLICADO\n\nEl código interno "' + editingProperty.codeInternal + '" ya existe en otra propiedad.\n\nPor favor elija otro código diferente.');
        }
        else if (error.response?.data) {
            const responseData = error.response.data;
            const errorString = JSON.stringify(responseData).toLowerCase();
            
            if (errorString.includes('código interno') || errorString.includes('codeinternal') || 
                errorString.includes('duplicado') || errorString.includes('ya existe')) {
                alert('❌ CÓDIGO DUPLICADO\n\nEl código interno "' + editingProperty.codeInternal + '" ya existe en otra propiedad.\n\nPor favor elija otro código diferente.');
            } 
            else if (responseData.title) {
                alert('❌ Error: ' + responseData.title);
            }
            else if (typeof responseData === 'string') {
                if (responseData.toLowerCase().includes('código interno') || 
                    responseData.toLowerCase().includes('codeinternal') ||
                    responseData.toLowerCase().includes('duplicado')) {
                    alert('❌ CÓDIGO DUPLICADO\n\nEl código interno "' + editingProperty.codeInternal + '" ya existe en otra propiedad.\n\nPor favor elija otro código diferente.');
                } else {
                    alert('❌ Error: ' + responseData);
                }
            }
            else {
                alert('❌ Error al editar la propiedad. Por favor intente nuevamente.');
            }
        }
        else {
            alert('❌ Error al editar la propiedad. Por favor intente nuevamente.');
        }
    }
};




    const handleCancelEdit = () => {
        setShowEditModal(false);
        setEditingProperty(null);
        setEditImageFile(null);
    };

    const handleDeleteClick = (property) => {
        setPropertyToDelete(property);
        setShowDeleteConfirm(true);
    };

    const handleConfirmDelete = async () => {
        try {
            if (propertyToDelete) {
                await axios.delete(`${API_URL_PROPERTIES}/${propertyToDelete.idProperty}`);
                fetchProperties();
                setShowDeleteConfirm(false);
                setPropertyToDelete(null);
            }
        } catch (error) {
            console.error('Error al eliminar la propiedad:', error);
        }
    };

    const handleCancelDelete = () => {
        setShowDeleteConfirm(false);
        setPropertyToDelete(null);
    };

    const getOwnerName = (idOwner) => {
        const owner = owners.find(o => o.idOwner === idOwner);
        return owner ? owner.name : 'Desconocido';
    };

    const handleManageImages = (property) => {
        setSelectedPropertyForImages(property);
        setShowImageManager(true);
    };

    const handleCloseImageManager = () => {
        setShowImageManager(false);
        setSelectedPropertyForImages(null);
        fetchProperties();
    };

    // --- Renderización del Componente ---
    return (
        <section className="container mx-auto p-6 bg-gray-100 min-h-screen">
            {/* Formulario para añadir nueva propiedad */}
            <Card className="mb-8 p-6 bg-white shadow-lg rounded-xl">
                <h2 className="text-2xl font-semibold mb-6 text-gray-800">Añadir Nueva Propiedad</h2>
                <form onSubmit={handleAddPropertySubmit} className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Nombre de la Propiedad *
                            </label>
                            <Input
                                type="text"
                                name="name"
                                value={newProperty.name}
                                onChange={handleNewPropertyChange}
                                placeholder="Ej. Casa familiar en la playa"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Dirección Completa *
                            </label>
                            <Input
                                type="text"
                                name="address"
                                value={newProperty.address}
                                onChange={handleNewPropertyChange}
                                placeholder="Ej. Calle 10 # 5-20, Ciudad"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Precio ($) *
                            </label>
                            <Input
                                type="number"
                                name="price"
                                value={newProperty.price}
                                onChange={handleNewPropertyChange}
                                placeholder="Ej. 150000"
                                step="0.01"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Código Interno *
                            </label>
                            <Input
                                type="text"
                                name="codeInternal"
                                value={newProperty.codeInternal}
                                onChange={handleNewPropertyChange}
                                placeholder="Ej. PRO-001"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Año de Construcción *
                            </label>
                            <Input
                                type="number"
                                name="year"
                                value={newProperty.year}
                                onChange={handleNewPropertyChange}
                                placeholder="Ej. 2020"
                                min="1900"
                                max="2100"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Propietario *
                            </label>
                            <Select
                                name="idOwner"
                                value={newProperty.idOwner}
                                onChange={handleNewPropertyChange}
                                required
                                className="w-full"
                            >
                                <option value="">Selecciona un propietario</option>
                                {owners.map(owner => (
                                    <option key={owner.idOwner} value={owner.idOwner}>
                                        {owner.name}
                                    </option>
                                ))}
                            </Select>
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Imagen de la Propiedad
                            </label>
                            <Input
                                type="file"
                                name="newImage"
                                onChange={handleNewImageChange}
                                accept="image/*"
                                className="w-full"
                            />
                            <p className="text-gray-500 text-xs mt-1">Formatos: JPG, PNG, GIF (Máx. 5MB)</p>
                        </div>
                    </div>

                    <div className="bg-blue-50 p-3 rounded-lg">
                        <p className="text-blue-800 text-sm">
                            * Campos obligatorios
                        </p>
                    </div>

                    <Button type="submit" className="w-full bg-blue-600 hover:bg-blue-700">
                        Añadir Propiedad
                    </Button>
                </form>
            </Card>

            {/* Sección de Filtros de Búsqueda */}
            <Card className="mb-8 p-6 bg-white shadow-lg rounded-xl">
                <h2 className="text-2xl font-semibold mb-6 text-gray-800">Filtrar Propiedades</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    <div>
                        <label className="block text-gray-700 font-medium mb-2 text-sm">
                            Filtrar por Nombre
                        </label>
                        <Input
                            type="text"
                            name="name"
                            value={filters.name}
                            onChange={handleFiltersChange}
                            placeholder="Buscar por nombre..."
                            className="w-full"
                        />
                    </div>
                    <div>
                        <label className="block text-gray-700 font-medium mb-2 text-sm">
                            Filtrar por Dirección
                        </label>
                        <Input
                            type="text"
                            name="address"
                            value={filters.address}
                            onChange={handleFiltersChange}
                            placeholder="Buscar por dirección..."
                            className="w-full"
                        />
                    </div>
                    <div>
                        <label className="block text-gray-700 font-medium mb-2 text-sm">
                            Precio Mínimo
                        </label>
                        <Input
                            type="number"
                            name="priceMin"
                            value={filters.priceMin}
                            onChange={handleFiltersChange}
                            placeholder="Ej. 100000"
                            className="w-full"
                        />
                    </div>
                    <div>
                        <label className="block text-gray-700 font-medium mb-2 text-sm">
                            Precio Máximo
                        </label>
                        <Input
                            type="number"
                            name="priceMax"
                            value={filters.priceMax}
                            onChange={handleFiltersChange}
                            placeholder="Ej. 500000"
                            className="w-full"
                        />
                    </div>
                </div>
            </Card>

            {/* Tabla de Propiedades */}
            <div className="overflow-x-auto bg-white rounded-xl shadow-lg">
                <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                        <tr>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Código Interno</th>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Imagen</th>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre</th>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Dirección</th>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Precio</th>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Propietario</th>
                            <th scope="col" className="py-3 px-6 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
                        </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                        {properties.length > 0 ? (
                            properties.map((property) => (
                                <tr key={property.idProperty} className="hover:bg-gray-50">
                                    <td className="py-4 px-6 whitespace-nowrap text-sm font-medium text-blue-800">
                                        {property.codeInternal}
                                    </td>
                                    <td className="py-4 px-6 whitespace-nowrap text-sm font-medium text-gray-900">
                                        {property.imageUrls && property.imageUrls.length > 0 ? (
                                            <img
                                                src={`http://localhost:50000${property.imageUrls[0]}`}
                                                alt={property.name}
                                                className="w-16 h-16 rounded-lg object-cover"
                                                onError={(e) => { e.target.onerror = null; e.target.src = "https://placehold.co/100x100/F0F0F0/333333?text=Sin+Imagen"; }}
                                            />
                                        ) : (
                                            <img
                                                src="https://placehold.co/100x100/F0F0F0/333333?text=Sin+Imagen"
                                                alt="Sin imagen"
                                                className="w-16 h-16 rounded-lg object-cover"
                                            />
                                        )}
                                    </td>
                                    <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">{property.name}</td>
                                    <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">{property.address}</td>
                                    <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">{`$${property.price.toFixed(2)}`}</td>
                                    <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">{getOwnerName(property.idOwner)}</td>
                                    <td className="py-4 px-6 whitespace-nowrap text-sm font-medium">
                                        <Button onClick={() => handleEditClick(property)} className="bg-yellow-500 hover:bg-yellow-600 text-white mr-2">
                                            Editar
                                        </Button>
                                        <Button onClick={() => handleManageImages(property)} className="bg-green-600 hover:bg-green-700 text-white mr-2">
                                            Imágenes
                                        </Button>
                                        <Button onClick={() => handleDeleteClick(property)} className="bg-red-600 hover:bg-red-700 text-white">
                                            Eliminar
                                        </Button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="7" className="py-4 px-4 text-center text-gray-500">
                                    No se encontraron propiedades.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {/* Modal de Edición MEJORADO */}
            {showEditModal && editingProperty && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-2xl mx-auto p-8">
                        <button
                            onClick={handleCancelEdit}
                            className="absolute top-4 right-4 text-gray-500 hover:text-gray-700 text-2xl"
                        >
                            ×
                        </button>

                        <h3 className="text-2xl font-semibold mb-6 text-gray-800 text-center">Editar Propiedad</h3>

                        <form onSubmit={handleEditSubmit} className="space-y-6">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

                                {/* Campo: Nombre */}
                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Nombre de la Propiedad *
                                    </label>
                                    <Input
                                        type="text"
                                        name="name"
                                        value={editingProperty.name}
                                        onChange={handleEditChange}
                                        placeholder="Ej. Casa familiar en la playa"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                {/* Campo: Dirección */}
                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Dirección Completa *
                                    </label>
                                    <Input
                                        type="text"
                                        name="address"
                                        value={editingProperty.address}
                                        onChange={handleEditChange}
                                        placeholder="Ej. Calle 10 # 5-20, Ciudad"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                {/* Campo: Precio */}
                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Precio ($) *
                                    </label>
                                    <Input
                                        type="number"
                                        name="price"
                                        value={editingProperty.price}
                                        onChange={handleEditChange}
                                        placeholder="Ej. 150000"
                                        step="0.01"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                {/* Campo: Código Interno */}
                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Código Interno *
                                    </label>
                                    <Input
                                        type="text"
                                        name="codeInternal"
                                        value={editingProperty.codeInternal}
                                        onChange={handleEditChange}
                                        placeholder="Ej. PRO-001"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                {/* Campo: Año */}
                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Año de Construcción *
                                    </label>
                                    <Input
                                        type="number"
                                        name="year"
                                        value={editingProperty.year}
                                        onChange={handleEditChange}
                                        placeholder="Ej. 2020"
                                        min="1900"
                                        max="2100"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                {/* Campo: Propietario */}
                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Propietario *
                                    </label>
                                    <Select
                                        name="idOwner"
                                        value={editingProperty.idOwner}
                                        onChange={handleEditChange}
                                        required
                                        className="w-full"
                                    >
                                        <option value="">Selecciona un propietario</option>
                                        {owners.map(owner => (
                                            <option key={owner.idOwner} value={owner.idOwner}>
                                                {owner.name}
                                            </option>
                                        ))}
                                    </Select>
                                </div>

                            </div>

                            {/* Información de campos obligatorios */}
                            <div className="bg-blue-50 p-3 rounded-lg">
                                <p className="text-blue-800 text-sm">
                                    * Campos obligatorios
                                </p>
                            </div>

                            <div className="flex justify-center space-x-4 mt-6">
                                <Button
                                    type="button"
                                    onClick={handleCancelEdit}
                                    className="bg-gray-500 hover:bg-gray-600 px-6 py-2"
                                >
                                    Cancelar
                                </Button>
                                <Button
                                    type="submit"
                                    className="bg-blue-600 hover:bg-blue-700 px-6 py-2"
                                >
                                    Guardar Cambios
                                </Button>
                            </div>
                        </form>
                    </Card>
                </div>
            )}

            {/* Modal de Confirmación de Eliminación */}
            {showDeleteConfirm && propertyToDelete && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-md mx-auto p-8 text-center">
                        <h3 className="text-2xl font-semibold mb-4 text-gray-800">Confirmar Eliminación</h3>
                        <p className="text-gray-700 mb-6">
                            ¿Estás seguro de que quieres eliminar la propiedad:
                            <br /><span className="font-bold">"{propertyToDelete.name}"</span> con ID: <span className="font-bold">{propertyToDelete.idProperty}</span>?
                            <br />Esta acción no se puede deshacer.
                        </p>
                        <div className="flex justify-center space-x-4 mt-6">
                            <Button type="button" onClick={handleCancelDelete} className="bg-gray-500 hover:bg-gray-600">Cancelar</Button>
                            <Button type="button" onClick={handleConfirmDelete} className="bg-red-600 hover:bg-red-700">Eliminar</Button>
                        </div>
                    </Card>
                </div>
            )}

            {/* Modal de Gestión de Imágenes */}
            {showImageManager && selectedPropertyForImages && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-4xl mx-auto p-8 max-h-[90vh] overflow-y-auto">
                        <button
                            onClick={handleCloseImageManager}
                            className="absolute top-4 right-4 text-gray-500 hover:text-gray-700 text-2xl"
                        >
                            ×
                        </button>
                        <PropertyImageManager
                            property={selectedPropertyForImages}
                            onClose={handleCloseImageManager}
                            apiUrl={API_URL_PROPERTIES}
                            apiUrlImages={API_URL_IMAGES}
                        />
                    </Card>
                </div>
            )}
        </section>
    );
};

export default PropertyManagement;
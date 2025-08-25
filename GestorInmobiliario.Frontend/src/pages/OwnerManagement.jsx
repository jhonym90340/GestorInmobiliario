import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Input, Card } from '../components/common.jsx';
import ImageWithFallback from '../components/ImageWithFallback';

const OwnerManagement = () => {
    const [owners, setOwners] = useState([]);
    const [newOwner, setNewOwner] = useState({
        name: '',
        address: '',
        birthday: ''
    });
    const [photoFile, setPhotoFile] = useState(null);
    const [selectedOwnerForImage, setSelectedOwnerForImage] = useState(null);
    const [showImageModal, setShowImageModal] = useState(false);
    const [showLargeImageModal, setShowLargeImageModal] = useState(false);
    const [selectedImage, setSelectedImage] = useState('');
    const [uploadingImage, setUploadingImage] = useState(false);

    // Estados para la funcionalidad de edición
    const [editingOwner, setEditingOwner] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);

    // Estados para la funcionalidad de eliminación
    const [ownerToDelete, setOwnerToDelete] = useState(null);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

    // URLs de las APIs 
    const API_URL_OWNERS = 'http://localhost:50000/api/owners';

    useEffect(() => {
        fetchOwners();
    }, []);

    const fetchOwners = async () => {
        try {
            const response = await axios.get(API_URL_OWNERS);
            setOwners(response.data);
            console.log("Propietarios obtenidos:", response.data);
        } catch (error) {
            console.error('Error al obtener los propietarios:', error);
        }
    };

    const handleNewOwnerChange = (e) => {
        const { name, value } = e.target;
        setNewOwner(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handlePhotoChange = (e) => {
        setPhotoFile(e.target.files[0]);
    };

    const handleAddOwnerSubmit = async (e) => {
        e.preventDefault();

        try {
            const formData = new FormData();
            formData.append('Name', newOwner.name);
            formData.append('Address', newOwner.address);
            if (newOwner.birthday) {
                formData.append('Birthday', new Date(newOwner.birthday).toISOString());
            }
            if (photoFile) {
                formData.append('PhotoFile', photoFile);
            }

            const response = await axios.post(
                `${API_URL_OWNERS}/with-image`,
                formData,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                }
            );

            console.log('Nuevo propietario creado:', response.data);
            await fetchOwners();

            // Reset form
            setNewOwner({ name: '', address: '', birthday: '' });
            setPhotoFile(null);
            document.getElementById('photo-input').value = '';

            alert('✅ Propietario creado exitosamente');

        } catch (error) {
            console.error('Error al añadir el propietario:', error);

            // Manejo mejorado de errores
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
                            case 'Birthday':
                                fieldName = 'Fecha de Nacimiento';
                                break;
                        }

                        if (errorMsg.includes('required') || errorMsg.includes('obligatorio')) {
                            friendlyMessage = 'es obligatorio';
                        } else if (errorMsg.includes('minimum length') || errorMsg.includes('mínimo')) {
                            friendlyMessage = 'es demasiado corto';
                        }

                        errorMessage += `• ${fieldName}: ${friendlyMessage}\n`;
                    });
                });

                alert(errorMessage);
            } else {
                alert('❌ Error al añadir propietario: ' + (error.response?.data?.title || error.message));
            }
        }
    };

    // Función para manejar la edición de propietarios
    const handleEditClick = (owner) => {
        setEditingOwner({
            ...owner,
            birthday: owner.birthday ? owner.birthday.split('T')[0] : '' // Formatear fecha para input date
        });
        setShowEditModal(true);
    };

    const handleEditChange = (e) => {
        const { name, value } = e.target;
        setEditingOwner(prevState => ({ ...prevState, [name]: value }));
    };

    const handleEditSubmit = async (e) => {
        e.preventDefault();
        try {
            const updatedOwner = {
                name: editingOwner.name,
                address: editingOwner.address,
                birthday: editingOwner.birthday ? new Date(editingOwner.birthday).toISOString() : null
            };

            await axios.put(`${API_URL_OWNERS}/${editingOwner.idOwner}`, updatedOwner);

            setShowEditModal(false);
            setEditingOwner(null);
            fetchOwners();

            alert('✅ Propietario actualizado exitosamente');

        } catch (error) {
            console.error('Error al editar el propietario:', error);

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
                            case 'Birthday':
                                fieldName = 'Fecha de Nacimiento';
                                break;
                        }

                        if (errorMsg.includes('required') || errorMsg.includes('obligatorio')) {
                            friendlyMessage = 'es obligatorio';
                        } else if (errorMsg.includes('minimum length') || errorMsg.includes('mínimo')) {
                            friendlyMessage = 'es demasiado corto';
                        }

                        errorMessage += `• ${fieldName}: ${friendlyMessage}\n`;
                    });
                });

                alert(errorMessage);
            } else {
                alert('❌ Error al editar propietario: ' + (error.response?.data?.title || error.message));
            }
        }
    };

    const handleCancelEdit = () => {
        setShowEditModal(false);
        setEditingOwner(null);
    };

    // Función para manejar la eliminación de propietarios
    const handleDeleteClick = (owner) => {
        setOwnerToDelete(owner);
        setShowDeleteConfirm(true);
    };

    const handleConfirmDelete = async () => {
        try {
            if (ownerToDelete) {
                await axios.delete(`${API_URL_OWNERS}/${ownerToDelete.idOwner}`);
                fetchOwners();
                setShowDeleteConfirm(false);
                setOwnerToDelete(null);
                alert('✅ Propietario eliminado exitosamente');
            }
        } catch (error) {
            console.error('Error al eliminar el propietario:', error);
            alert('❌ Error al eliminar propietario: ' + (error.response?.data?.title || error.message));
        }
    };

    const handleCancelDelete = () => {
        setShowDeleteConfirm(false);
        setOwnerToDelete(null);
    };

    const handleUploadOwnerImage = async (ownerId, imageFile) => {
        setUploadingImage(true);
        try {
            const formData = new FormData();
            formData.append('imageFile', imageFile);

            const response = await axios.post(
                `${API_URL_OWNERS}/${ownerId}/upload-image`,
                formData,
                { headers: { 'Content-Type': 'multipart/form-data' } }
            );

            console.log('Imagen subida exitosamente:', response.data);
            await fetchOwners();
            setShowImageModal(false);
            setSelectedOwnerForImage(null);

        } catch (error) {
            console.error('Error al subir imagen:', error);
            alert('❌ Error al subir imagen: ' + (error.response?.data?.title || error.message));
        } finally {
            setUploadingImage(false);
        }
    };

    const handleImageUpload = (event, ownerId) => {
        const file = event.target.files[0];
        if (file) {
            handleUploadOwnerImage(ownerId, file);
        }
    };

    const handleManageImages = (owner) => {
        setSelectedOwnerForImage(owner);
        setShowImageModal(true);
    };

    const handleViewLargeImage = (imageUrl) => {
        setSelectedImage(imageUrl);
        setShowLargeImageModal(true);
    };

    const handleCloseImageModal = () => {
        setShowImageModal(false);
        setSelectedOwnerForImage(null);
    };

    const handleCloseLargeImageModal = () => {
        setShowLargeImageModal(false);
        setSelectedImage('');
    };

    // Función para construir URL completa de imagen
    const getImageUrl = (url) => {
        if (!url) return '';
        if (url.startsWith('http')) return url;
        if (url.startsWith('/')) return `http://localhost:50000${url}`;
        return `http://localhost:50000/images/${url}`;
    };

    return (
        <section className="container mx-auto p-6 bg-gray-100 min-h-screen">
            <h1 className="text-4xl font-extrabold text-gray-900 mb-8 text-center">Gestión de Propietarios</h1>

            {/* Formulario para añadir nuevo propietario */}
            <Card className="mb-8 p-6 bg-white shadow-lg rounded-xl">
                <h2 className="text-2xl font-semibold mb-6 text-gray-800">Añadir Nuevo Propietario</h2>
                <form onSubmit={handleAddOwnerSubmit} className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Nombre Completo *
                            </label>
                            <Input
                                type="text"
                                name="name"
                                value={newOwner.name}
                                onChange={handleNewOwnerChange}
                                placeholder="Ej. Juan Pérez García"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Dirección *
                            </label>
                            <Input
                                type="text"
                                name="address"
                                value={newOwner.address}
                                onChange={handleNewOwnerChange}
                                placeholder="Ej. Avenida Principal 123, Ciudad"
                                required
                                className="w-full"
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Fecha de Nacimiento
                            </label>
                            <input
                                type="date"
                                name="birthday"
                                value={newOwner.birthday}
                                onChange={handleNewOwnerChange}
                                className="w-full p-3 border border-gray-300 rounded-lg"
                            />
                            <p className="text-gray-500 text-xs mt-1">Opcional</p>
                        </div>

                        <div>
                            <label className="block text-gray-700 font-medium mb-2 text-sm">
                                Foto del Propietario
                            </label>
                            <input
                                id="photo-input"
                                type="file"
                                accept="image/*"
                                onChange={handlePhotoChange}
                                className="w-full p-3 border border-gray-300 rounded-lg"
                            />
                            {photoFile && (
                                <p className="text-green-600 text-sm mt-1">
                                    Archivo seleccionado: {photoFile.name}
                                </p>
                            )}
                            <p className="text-gray-500 text-xs mt-1">Formatos: JPG, PNG (Máx. 5MB)</p>
                        </div>
                    </div>

                    <div className="bg-blue-50 p-3 rounded-lg">
                        <p className="text-blue-800 text-sm">
                            * Campos obligatorios
                        </p>
                    </div>

                    <Button type="submit" className="w-full bg-blue-600 hover:bg-blue-700">
                        Añadir Propietario
                    </Button>
                </form>
            </Card>

            {/* Lista de Propietarios */}
            <Card className="p-6 bg-white shadow-lg rounded-xl">
                <h2 className="text-2xl font-semibold mb-6 text-gray-800">Lista de Propietarios</h2>

                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Foto</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Dirección</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha Nacimiento</th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {owners.map((owner) => (
                                <tr key={owner.idOwner} className="hover:bg-gray-50">
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div
                                            className="w-12 h-12 rounded-full object-cover cursor-pointer transition-transform hover:scale-110"
                                            onClick={() => handleViewLargeImage(owner.photo)}
                                        >
                                            <ImageWithFallback
                                                src={owner.photo}
                                                alt={owner.name}
                                                className="w-12 h-12 rounded-full object-cover"
                                                fallbackSrc="https://placehold.co/48x48/F0F0F0/333333?text=Sin+Foto"
                                            />
                                        </div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{owner.name}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{owner.address}</td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                                        {owner.birthday ? new Date(owner.birthday).toLocaleDateString() : 'N/A'}
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                                        <div className="flex space-x-2">
                                            <Button
                                                onClick={() => handleEditClick(owner)}
                                                className="bg-yellow-500 hover:bg-yellow-600 text-white text-xs px-3 py-1"
                                            >
                                                Editar
                                            </Button>
                                            <Button
                                                onClick={() => handleManageImages(owner)}
                                                className="bg-green-600 hover:bg-green-700 text-white text-xs px-3 py-1"
                                            >
                                                Foto
                                            </Button>
                                            <Button
                                                onClick={() => handleDeleteClick(owner)}
                                                className="bg-red-600 hover:bg-red-700 text-white text-xs px-3 py-1"
                                            >
                                                Eliminar
                                            </Button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </Card>

            {/* Modal de Edición de Propietario */}
            {showEditModal && editingOwner && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-2xl mx-auto p-8">
                        <button
                            onClick={handleCancelEdit}
                            className="absolute top-4 right-4 text-gray-500 hover:text-gray-700 text-2xl"
                        >
                            ×
                        </button>

                        <h3 className="text-2xl font-semibold mb-6 text-gray-800 text-center">Editar Propietario</h3>

                        <form onSubmit={handleEditSubmit} className="space-y-6">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Nombre Completo *
                                    </label>
                                    <Input
                                        type="text"
                                        name="name"
                                        value={editingOwner.name}
                                        onChange={handleEditChange}
                                        placeholder="Ej. Juan Pérez García"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Dirección *
                                    </label>
                                    <Input
                                        type="text"
                                        name="address"
                                        value={editingOwner.address}
                                        onChange={handleEditChange}
                                        placeholder="Ej. Avenida Principal 123, Ciudad"
                                        required
                                        className="w-full"
                                    />
                                </div>

                                <div>
                                    <label className="block text-gray-700 font-medium mb-2 text-sm">
                                        Fecha de Nacimiento
                                    </label>
                                    <input
                                        type="date"
                                        name="birthday"
                                        value={editingOwner.birthday}
                                        onChange={handleEditChange}
                                        className="w-full p-3 border border-gray-300 rounded-lg"
                                    />
                                    <p className="text-gray-500 text-xs mt-1">Opcional</p>
                                </div>

                            </div>

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
            {showDeleteConfirm && ownerToDelete && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-md mx-auto p-8 text-center">
                        <h3 className="text-2xl font-semibold mb-4 text-gray-800">Confirmar Eliminación</h3>
                        <p className="text-gray-700 mb-6">
                            ¿Estás seguro de que quieres eliminar al propietario:
                            <br /><span className="font-bold">"{ownerToDelete.name}"</span>?
                            <br />Esta acción no se puede deshacer.
                        </p>
                        <div className="flex justify-center space-x-4 mt-6">
                            <Button type="button" onClick={handleCancelDelete} className="bg-gray-500 hover:bg-gray-600">Cancelar</Button>
                            <Button type="button" onClick={handleConfirmDelete} className="bg-red-600 hover:bg-red-700">Eliminar</Button>
                        </div>
                    </Card>
                </div>
            )}

            {/* Modal para subir/cambiar imagen */}
            {showImageModal && selectedOwnerForImage && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-md mx-auto p-8">
                        <button
                            onClick={handleCloseImageModal}
                            className="absolute top-4 right-4 text-gray-500 hover:text-gray-700 text-2xl"
                        >
                            ×
                        </button>

                        <h3 className="text-2xl font-semibold mb-6 text-gray-800 text-center">
                            {selectedOwnerForImage.photo ? 'Cambiar Foto' : 'Subir Foto'} para: {selectedOwnerForImage.name}
                        </h3>

                        <div className="mb-6 flex justify-center">
                            <ImageWithFallback
                                src={selectedOwnerForImage.photo}
                                alt={selectedOwnerForImage.name}
                                className="w-32 h-32 rounded-full object-cover border-4 border-gray-300"
                                fallbackSrc="https://placehold.co/128x128/F0F0F0/333333?text=Sin+Foto"
                            />
                        </div>

                        <input
                            type="file"
                            accept="image/*"
                            onChange={(e) => handleImageUpload(e, selectedOwnerForImage.idOwner)}
                            disabled={uploadingImage}
                            className="w-full p-3 border border-gray-300 rounded-lg mb-4"
                        />

                        {uploadingImage && (
                            <p className="text-blue-600 text-center">Subiendo imagen...</p>
                        )}

                        <div className="flex justify-center mt-6">
                            <Button
                                onClick={handleCloseImageModal}
                                className="bg-gray-500 hover:bg-gray-600"
                            >
                                Cancelar
                            </Button>
                        </div>
                    </Card>
                </div>
            )}

            {/* Modal para ver foto ampliada */}
            {showLargeImageModal && (
                <div className="fixed inset-0 bg-black bg-opacity-90 flex items-center justify-center p-4 z-50">
                    <div className="relative max-w-4xl max-h-full">
                        <button
                            onClick={handleCloseLargeImageModal}
                            className="absolute top-4 right-4 text-white hover:text-gray-300 text-3xl z-10 bg-gray-800 rounded-full p-2"
                        >
                            ×
                        </button>

                        <div className="bg-white p-2 rounded-lg">
                            <img
                                src={getImageUrl(selectedImage)}
                                alt="Foto ampliada"
                                className="max-w-full max-h-96 object-contain rounded-lg"
                                onError={(e) => {
                                    e.target.onerror = null;
                                    e.target.src = "https://placehold.co/600x400/F0F0F0/333333?text=Imagen+No+Disponible";
                                }}
                            />
                        </div>

                        <div className="text-center mt-4">
                            <Button
                                onClick={handleCloseLargeImageModal}
                                className="bg-gray-600 hover:bg-gray-700 text-white"
                            >
                                Cerrar
                            </Button>
                        </div>
                    </div>
                </div>
            )}
        </section>
    );
};

export default OwnerManagement;
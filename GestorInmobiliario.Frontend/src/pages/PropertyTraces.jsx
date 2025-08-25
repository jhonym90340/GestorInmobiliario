import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, Input, Select, Card } from '../components/common.jsx';

const PropertyTraces = () => {
    const [propertyTraces, setPropertyTraces] = useState([]);
    const [allProperties, setAllProperties] = useState([]);
    const [allOwners, setAllOwners] = useState([]);
    const [newTrace, setNewTrace] = useState({
        dateSale: '',
        name: '',
        value: '',
        tax: '',
        idProperty: '',
    });

    // Estados para la funcionalidad de edición
    const [editingTrace, setEditingTrace] = useState(null);
    const [showEditModal, setShowEditModal] = useState(false);

    // Estados para la funcionalidad de eliminación
    const [traceToDelete, setTraceToDelete] = useState(null);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

    // URLs de las APIs
    const API_URL_PROPERTY_TRACES = 'http://localhost:50000/api/propertytraces';
    const API_URL_PROPERTIES = 'http://localhost:50000/api/properties';
    const API_URL_OWNERS = 'http://localhost:50000/api/owners';

    // useEffect para cargar los registros de trazabilidad, propiedades y propietarios al montar
    useEffect(() => {
        fetchPropertyTraces();
        fetchAllProperties();
        fetchAllOwners();
    }, []);

    // Función para obtener registros de trazabilidad
    const fetchPropertyTraces = async () => {
        try {
            const response = await axios.get(API_URL_PROPERTY_TRACES);
            setPropertyTraces(response.data);
            console.log('Registros de trazabilidad obtenidos:', response.data);
        } catch (error) {
            console.error('Error al obtener los registros de trazabilidad:', error);
        }
    };

    // Función para obtener todas las propiedades
    const fetchAllProperties = async () => {
        try {
            const response = await axios.get(API_URL_PROPERTIES);
            setAllProperties(response.data);
            console.log('Todas las propiedades obtenidas:', response.data);
        } catch (error) {
            console.error('Error al obtener todas las propiedades:', error);
        }
    };

    // Función para obtener todos los propietarios
    const fetchAllOwners = async () => {
        try {
            const response = await axios.get(API_URL_OWNERS);
            setAllOwners(response.data);
            console.log('Todos los propietarios obtenidos:', response.data);
        } catch (error) {
            console.error('Error al obtener todos los propietarios:', error);
        }
    };

    const handleNewTraceChange = (e) => {
        const { name, value } = e.target;
        setNewTrace(prev => ({ ...prev, [name]: value }));
    };

    const handleAddTraceSubmit = async (e) => {
        e.preventDefault();

        console.log('Intentando añadir nuevo registro de trazabilidad...');
        try {
            const traceDataToSend = {
                dateSale: newTrace.dateSale,
                name: newTrace.name,
                value: parseFloat(newTrace.value),
                tax: parseFloat(newTrace.tax),
                idProperty: newTrace.idProperty,
            };

            console.log('Datos a enviar:', traceDataToSend);

            const response = await axios.post(API_URL_PROPERTY_TRACES, traceDataToSend);
            console.log('Nuevo registro de traza añadido con éxito:', response.data);

            await fetchPropertyTraces();
            setNewTrace({ dateSale: '', name: '', value: '', tax: '', idProperty: '' });

        } catch (error) {
            console.error('Error al añadir el registro de trazabilidad:', error);
            if (error.response && error.response.data && error.response.data.errors) {
                console.error('Errores de validación del backend:', error.response.data.errors);
                alert('Error al añadir traza: ' + JSON.stringify(error.response.data.errors, null, 2));
            } else {
                alert('Error al añadir traza. Ver consola para más detalles.');
            }
        }
    };

    // --- Funcionalidad de Edición ---
    const handleEditClick = (trace) => {
        setEditingTrace({
            ...trace,
            dateSale: trace.dateSale ? new Date(trace.dateSale).toISOString().split('T')[0] : '',
            value: trace.value.toString(),
            tax: trace.tax.toString(),
        });
        setShowEditModal(true);
    };

    const handleEditFormChange = (e) => {
        const { name, value } = e.target;
        setEditingTrace(prev => ({ ...prev, [name]: value }));
    };

    const handleUpdateTraceSubmit = async (e) => {
        e.preventDefault();
        if (!editingTrace || !editingTrace.idPropertyTrace) return;

        try {
            const traceDataToUpdate = {
                idPropertyTrace: editingTrace.idPropertyTrace,
                dateSale: editingTrace.dateSale,
                name: editingTrace.name,
                value: parseFloat(editingTrace.value),
                tax: parseFloat(editingTrace.tax),
                idProperty: editingTrace.idProperty,
            };

            await axios.put(`${API_URL_PROPERTY_TRACES}/${editingTrace.idPropertyTrace}`, traceDataToUpdate);
            console.log('Registro de traza actualizado con éxito:', editingTrace.idPropertyTrace);

            await fetchPropertyTraces();
            setShowEditModal(false);
            setEditingTrace(null);

        } catch (error) {
            console.error('Error al actualizar el registro de trazabilidad:', error);
            if (error.response && error.response.data && error.response.data.errors) {
                console.error('Errores de validación del backend:', error.response.data.errors);
                alert('Error al actualizar traza: ' + JSON.stringify(error.response.data.errors, null, 2));
            } else {
                alert('Error al actualizar traza. Ver consola para más detalles.');
            }
        }
    };

    const handleCancelEdit = () => {
        setShowEditModal(false);
        setEditingTrace(null);
    };

    // --- Funcionalidad de Eliminación ---
    const handleDeleteClick = (trace) => {
        setTraceToDelete(trace);
        setShowDeleteConfirm(true);
    };

    const handleConfirmDelete = async () => {
        if (!traceToDelete || !traceToDelete.idPropertyTrace) return;

        try {
            await axios.delete(`${API_URL_PROPERTY_TRACES}/${traceToDelete.idPropertyTrace}`);
            console.log('Registro de traza eliminado con éxito:', traceToDelete.idPropertyTrace);

            await fetchPropertyTraces();
            setShowDeleteConfirm(false);
            setTraceToDelete(null);

        } catch (error) {
            console.error('Error al eliminar el registro de trazabilidad:', error);
            if (error.response && error.response.data && error.response.data.errors) {
                console.error('Errores del backend al eliminar:', error.response.data.errors);
                alert('Error al eliminar traza: ' + JSON.stringify(error.response.data.errors, null, 2));
            } else {
                alert('Error al eliminar traza. Ver consola para más detalles.');
            }
        }
    };

    const handleCancelDelete = () => {
        setShowDeleteConfirm(false);
        setTraceToDelete(null);
    };

    // Función para obtener el nombre del propietario
    const getOwnerName = (idOwner) => {
        const owner = allOwners.find(o => o.idOwner === idOwner);
        return owner ? owner.name : 'N/A';
    };

    // Función para obtener el nombre de la propiedad
    const getPropertyName = (propertyId) => {
        const property = allProperties.find(p => p.idProperty === propertyId);
        return property ? `${property.codeInternal} - ${property.name}` : 'N/A';
    };

    // Función para obtener la propiedad asociada
    const getAssociatedProperty = (propertyId) => {
        return allProperties.find(p => p.idProperty === propertyId);
    };

    return (
        <section className="py-16 md:py-20 px-4 md:px-12 max-w-7xl mx-auto">
            <h2 className="text-4xl md:text-5xl font-bold text-center mb-12 text-gray-800">
                Gestionar Trazabilidad de Propiedades
            </h2>

            {/* Formulario para añadir nuevos registros de trazabilidad */}
            <Card className="max-w-xl mx-auto p-8 md:p-10 mb-16">
                <h3 className="text-2xl font-semibold text-center mb-8 text-gray-800">
                    Añadir Nuevo Registro de Trazabilidad
                </h3>
                <form onSubmit={handleAddTraceSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
                    <div>
                        <label className="block text-gray-700 font-medium mb-2 text-sm">
                            Nombre de la Propiedad *
                        </label>
                        <Input
                            type="date"
                            id="traceDateSale"
                            name="dateSale"
                            value={newTrace.dateSale}
                            onChange={handleNewTraceChange}
                            required
                        />
                    </div>
                    <div className="col-span-1">
                        <label htmlFor="traceName" className="block text-gray-700 font-medium mb-2 text-sm">
                            Descripción del Evento *
                        </label>
                        <Input
                            type="text"
                            id="traceName"
                            name="name"
                            value={newTrace.name}
                            onChange={handleNewTraceChange}
                            placeholder="Ej. Venta a comprador final, Revisión de precio"
                            required
                        />
                    </div>
                    <div className="col-span-1">
                        <label htmlFor="traceValue" className="block text-gray-700 font-medium mb-2 text-sm">
                            Valor ($) *
                        </label>
                        <Input
                            type="number"
                            id="traceValue"
                            name="value"
                            value={newTrace.value}
                            onChange={handleNewTraceChange}
                            placeholder="Ej. 1500000"
                            min="0"
                            step="0.01"
                            required
                        />
                    </div>
                    <div className="col-span-1">
                        <label htmlFor="traceTax" className="block text-gray-700 font-medium mb-2 text-sm">
                            Impuesto ($) *
                        </label>
                        <Input
                            type="number"
                            id="traceTax"
                            name="tax"
                            value={newTrace.tax}
                            onChange={handleNewTraceChange}
                            placeholder="Ej. 75000"
                            min="0"
                            step="0.01"
                            required
                        />
                    </div>
                    <div className="col-span-full">
                        <label htmlFor="traceIdProperty" className="block text-gray-700 font-medium mb-2 text-sm">
                            Propiedad Asociada *
                        </label>
                        <Select
                            id="traceIdProperty"
                            name="idProperty"
                            value={newTrace.idProperty}
                            onChange={handleNewTraceChange}
                            required
                            className="w-full"
                        >
                            <option value="">Selecciona una propiedad</option>
                            {allProperties.map(property => (
                                <option key={property.idProperty} value={property.idProperty}>
                                    {property.codeInternal} - {property.name}
                                </option>
                            ))}
                        </Select>
                        <p className="text-gray-500 text-xs mt-1">
                            Selecciona la propiedad a la que pertenece este registro
                        </p>
                    </div>
                    <div className="col-span-full mt-4">
                        <Button type="submit" className="w-full">
                            Añadir Registro de Traza
                        </Button>
                    </div>
                </form>
            </Card>

            {/* Listado de registros de trazabilidad en tabla */}
            <div className="py-8 md:py-12">
                <h3 className="text-3xl md:text-4xl font-bold text-center mb-10 text-gray-800">
                    Historial de Trazabilidad
                </h3>
                <div className="overflow-x-auto rounded-lg shadow-md">
                    <table className="min-w-full bg-white divide-y divide-gray-200">
                        <thead className="bg-gradient-to-r from-indigo-700 to-purple-600 text-white">
                            <tr>

                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Fecha Evento</th>
                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Descripción</th>
                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Valor</th>
                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Impuesto</th>
                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Propiedad</th>
                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Propietario</th>
                                <th className="py-3 px-6 text-left text-xs font-semibold uppercase tracking-wider">Acciones</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200">
                            {propertyTraces.map((trace) => {
                                const associatedProperty = getAssociatedProperty(trace.idProperty);
                                const associatedOwner = associatedProperty ? allOwners.find(o => o.idOwner === associatedProperty.idOwner) : null;

                                return (
                                    <tr key={trace.idPropertyTrace || trace.id || trace._id} className="hover:bg-gray-50 transition duration-150">

                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">
                                            {new Date(trace.dateSale).toLocaleDateString()}
                                        </td>
                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">{trace.name}</td>
                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">$ {parseFloat(trace.value).toLocaleString('es-ES', { minimumFractionDigits: 2 })}</td>
                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">$ {parseFloat(trace.tax).toLocaleString('es-ES', { minimumFractionDigits: 2 })}</td>
                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">
                                            {getPropertyName(trace.idProperty)}
                                        </td>
                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">
                                            {associatedOwner ? associatedOwner.name : 'N/A'}
                                        </td>
                                        <td className="py-4 px-6 whitespace-nowrap text-sm text-gray-800">
                                            <Button
                                                onClick={() => handleEditClick(trace)}
                                                className="px-3 py-1 text-xs bg-blue-500 hover:bg-blue-600 mr-2"
                                            >
                                                Editar
                                            </Button>
                                            <Button
                                                onClick={() => handleDeleteClick(trace)}
                                                className="px-3 py-1 text-xs bg-red-500 hover:bg-red-600"
                                            >
                                                Eliminar
                                            </Button>
                                        </td>
                                    </tr>
                                );
                            })}
                            {propertyTraces.length === 0 && (
                                <tr>
                                    <td colSpan="8" className="py-4 px-4 text-center text-gray-500">
                                        No hay registros de trazabilidad para mostrar.
                                    </td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>

            {/* Modal de Edición de Registro de Trazabilidad */}
            {showEditModal && editingTrace && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-2xl mx-auto p-8">
                        <h3 className="text-2xl font-semibold text-center mb-8 text-gray-800">Editar Registro de Trazabilidad</h3>
                        <form onSubmit={handleUpdateTraceSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-x-6 gap-y-4">
                            <div className="col-span-1">
                                <label htmlFor="editTraceDateSale" className="block text-gray-700 font-medium mb-2 text-sm">
                                    Fecha del Evento *
                                </label>
                                <Input
                                    type="date"
                                    id="editTraceDateSale"
                                    name="dateSale"
                                    value={editingTrace.dateSale}
                                    onChange={handleEditFormChange}
                                    required
                                />
                            </div>
                            <div className="col-span-1">
                                <label htmlFor="editTraceName" className="block text-gray-700 font-medium mb-2 text-sm">
                                    Descripción del Evento *
                                </label>
                                <Input
                                    type="text"
                                    id="editTraceName"
                                    name="name"
                                    value={editingTrace.name}
                                    onChange={handleEditFormChange}
                                    required
                                />
                            </div>
                            <div className="col-span-1">
                                <label htmlFor="editTraceValue" className="block text-gray-700 font-medium mb-2 text-sm">
                                    Valor ($) *
                                </label>
                                <Input
                                    type="number"
                                    id="editTraceValue"
                                    name="value"
                                    value={editingTrace.value}
                                    onChange={handleEditFormChange}
                                    min="0"
                                    step="0.01"
                                    required
                                />
                            </div>
                            <div className="col-span-1">
                                <label htmlFor="editTraceTax" className="block text-gray-700 font-medium mb-2 text-sm">
                                    Impuesto ($) *
                                </label>
                                <Input
                                    type="number"
                                    id="editTraceTax"
                                    name="tax"
                                    value={editingTrace.tax}
                                    onChange={handleEditFormChange}
                                    min="0"
                                    step="0.01"
                                    required
                                />
                            </div>
                            <div className="col-span-full">
                                <label htmlFor="editTraceIdProperty" className="block text-gray-700 font-medium mb-2 text-sm">
                                    Propiedad Asociada *
                                </label>
                                <Select
                                    id="editTraceIdProperty"
                                    name="idProperty"
                                    value={editingTrace.idProperty}
                                    onChange={handleEditFormChange}
                                    required
                                    className="w-full"
                                >
                                    <option value="">Selecciona una propiedad</option>
                                    {allProperties.map(property => (
                                        <option key={property.idProperty} value={property.idProperty}>
                                            {property.codeInternal} - {property.name}
                                        </option>
                                    ))}
                                </Select>
                            </div>
                            <div className="col-span-full flex justify-end space-x-4 mt-6">
                                <Button type="button" onClick={handleCancelEdit} className="bg-gray-500 hover:bg-gray-600">Cancelar</Button>
                                <Button type="submit" className="bg-green-600 hover:bg-green-700">Guardar Cambios</Button>
                            </div>
                        </form>
                    </Card>
                </div>
            )}

            {/* Modal de Confirmación de Eliminación */}
            {showDeleteConfirm && traceToDelete && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-75 flex items-center justify-center p-4 z-50">
                    <Card className="relative w-full max-w-md mx-auto p-8 text-center">
                        <h3 className="text-2xl font-semibold mb-4 text-gray-800">Confirmar Eliminación</h3>
                        <p className="text-gray-700 mb-6">
                            ¿Estás seguro de que quieres eliminar el registro de trazabilidad:
                            <br /><span className="font-bold">"{traceToDelete.name}"</span> con ID: <span className="font-bold">{traceToDelete.idPropertyTrace || traceToDelete.id}</span>?
                            <br />Esta acción no se puede deshacer.
                        </p>
                        <div className="flex justify-center space-x-4 mt-6">
                            <Button type="button" onClick={handleCancelDelete} className="bg-gray-500 hover:bg-gray-600">Cancelar</Button>
                            <Button type="button" onClick={handleConfirmDelete} className="bg-red-600 hover:bg-red-700">Eliminar</Button>
                        </div>
                    </Card>
                </div>
            )}
        </section>
    );
};

export default PropertyTraces;
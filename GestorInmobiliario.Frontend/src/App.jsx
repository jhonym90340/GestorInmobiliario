import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import Home from './pages/Home.jsx';
import PropertyManagement from './pages/PropertyManagement.jsx';
import OwnerManagement from './pages/OwnerManagement.jsx';
import PropertyTraces from './pages/PropertyTraces.jsx';
import axios from 'axios';

const App = () => {

    
    const [allPropertiesForOtherModules, setAllPropertiesForOtherModules] = useState([]);
    const [allOwnersForOtherModules, setAllOwnersForOtherModules] = useState([]);

    const API_URL_PROPERTIES = 'http://localhost:50000/api/properties';
    const API_URL_OWNERS = 'http://localhost:50000/api/owners';

    useEffect(() => {
        const fetchAllProperties = async () => {
            try {
                const response = await axios.get(API_URL_PROPERTIES);
                setAllPropertiesForOtherModules(response.data);
                console.log('Propiedades cargadas en App.jsx para otros módulos:', response.data);
            } catch (error) {
                console.error('Error al cargar propiedades en App.jsx:', error);
            }
        };

        const fetchAllOwners = async () => {
            try {
                const response = await axios.get(API_URL_OWNERS);
                setAllOwnersForOtherModules(response.data);
                console.log('Propietarios cargados en App.jsx para otros módulos:', response.data);
            } catch (error) {
                console.error('Error al cargar propietarios en App.jsx:', error);
            }
        };

        
        fetchAllProperties();
        fetchAllOwners();
    }, []); // El array de dependencias vacío asegura que se ejecute solo una vez al montar el componente

    return (
        <Router>
            <div className="flex flex-col min-h-screen bg-gray-100">
                {/* Navbar */}
                <nav className="fixed w-full top-0 left-0 bg-indigo-700 shadow-md p-4 z-50">
                    <div className="container mx-auto flex justify-between items-center">
                        <Link to="/" className="text-white text-3xl font-bold">
                            PropertyMaster
                        </Link>
                        <div className="flex space-x-6">
                            <Link to="/" className="text-white text-lg hover:text-blue-200 transition duration-300 font-medium">
                                Home
                            </Link>
                            <Link to="/properties" className="text-white text-lg hover:text-blue-200 transition duration-300 font-medium">
                                Property Management
                            </Link>
                            <Link to="/owners" className="text-white text-lg hover:text-blue-200 transition duration-300 font-medium">
                                Owner Management
                            </Link>
                            <Link to="/traces" className="text-white text-lg hover:text-blue-200 transition duration-300 font-medium">
                                Property Traces
                            </Link>
                        </div>
                    </div>
                </nav>

                {/* Contenido principal con padding superior para el navbar */}
                <main className="pt-20">
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/properties" element={<PropertyManagement />} />
                        <Route path="/owners" element={<OwnerManagement properties={allPropertiesForOtherModules} />} />
                        <Route path="/traces" element={<PropertyTraces properties={allPropertiesForOtherModules} />} />
                    </Routes>
                </main>

                {/* Footer (opcional) */}
                <footer className="bg-gray-800 text-white text-center py-6 mt-12">
                    <div className="container mx-auto">
                        <p>&copy; {new Date().getFullYear()} PropertyMaster. All rights reserved.</p> {/* ¡CAMBIO AQUÍ TAMBIÉN! */}
                    </div>
                </footer>
            </div>
        </Router>
    );
};

export default App;
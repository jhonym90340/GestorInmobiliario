import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import axios from 'axios';
import { BrowserRouter } from 'react-router-dom';
import Home from '../src/pages/Home';

// Mockear la librería axios para simular llamadas a la API
jest.mock('axios');

// Mockear useNavigate para evitar errores de React Router en las pruebas
const mockedUsedNavigate = jest.fn();
jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockedUsedNavigate,
}));

describe('Home Component', () => {
    // Limpiar mocks antes de cada prueba
    beforeEach(() => {
        jest.clearAllMocks();
    });

    test('should fetch and display recent properties on initial load', async () => {
        const mockProperties = [
            { idProperty: "1", name: "Villa Sol", address: "Calle 1", price: 250000, year: 2010, codeInternal: "VS001", idOwner: "O1", photos: ["/images/villa1.jpg"] },
            { idProperty: "2", name: "Apartamento Vista", address: "Avenida 2", price: 180000, year: 2015, codeInternal: "AV002", idOwner: "O2", photos: ["/images/apt1.jpg"] }
        ];

        axios.get.mockResolvedValue({ data: mockProperties });

        // Renderiza el componente dentro de BrowserRouter
        render(
            <BrowserRouter>
                <Home />
            </BrowserRouter>
        );

        // Usa await waitFor para esperar que los datos se carguen y el DOM se actualice
        await waitFor(() => {
            expect(screen.getByText(/Nuestras Últimas Propiedades/i)).toBeInTheDocument();
            expect(screen.getByText('Villa Sol')).toBeInTheDocument();
            expect(screen.getByText('Apartamento Vista')).toBeInTheDocument();
        });
    });

    test('should handle API errors gracefully', async () => {
        axios.get.mockRejectedValue(new Error('Network Error'));

        render(
            <BrowserRouter>
                <Home />
            </BrowserRouter>
        );

        // Esperar a que la pantalla muestre el mensaje de error o la ausencia de propiedades
        await waitFor(() => {
            expect(screen.getByText(/No hay propiedades disponibles/i)).toBeInTheDocument();
        });

        // Asegurarse de que no se muestre ninguna propiedad de prueba
        expect(screen.queryByText('Villa Sol')).not.toBeInTheDocument();
        expect(screen.queryByText('Apartamento Vista')).not.toBeInTheDocument();
    });
});
import React from 'react';
import { render, screen, waitFor, fireEvent, act } from '@testing-library/react';
import axios from 'axios';
import OwnerManagement from '../src/pages/OwnerManagement';

jest.mock('axios');

describe('OwnerManagement Component', () => {
    let originalError;

    beforeAll(() => {
        originalError = console.error;
        console.error = jest.fn();
    });

    afterAll(() => {
        console.error = originalError;
    });

    beforeEach(() => {
        jest.clearAllMocks();
        console.error.mockClear();
    });

    test('should fetch and display owners on initial load', async () => {
        const mockOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
            { idOwner: '2', name: 'Jane Smith', address: '456 Elm St', birthday: '1990-05-15', photo: '/images/owner2.jpg' },
        ];
        axios.get.mockResolvedValue({ data: mockOwners });

        render(<OwnerManagement />);

        await waitFor(() => {
            expect(screen.getByText('John Doe')).toBeInTheDocument();
            expect(screen.getByText('Jane Smith')).toBeInTheDocument();
        }, { timeout: 3000 });
    });

    test('should allow a new owner to be added', async () => {
        const mockInitialOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
        ];

        axios.get.mockResolvedValueOnce({ data: mockInitialOwners });

        const newOwner = { idOwner: '2', name: 'Peter Pan', address: 'Neverland', birthday: '1995-12-25', photo: '/images/peter.jpg' };
        axios.post.mockResolvedValue({ data: newOwner });

        render(<OwnerManagement />);

        // Esperar a que se carguen los datos iniciales
        await screen.findByText('John Doe');

        // Buscar los inputs por placeholder (campos obligatorios)
        const nameInput = screen.getByPlaceholderText(/Juan Pérez García/i);
        const addressInput = screen.getByPlaceholderText(/Avenida Principal 123, Ciudad/i);

        // Buscar el botón
        const createButton = screen.getByText('Añadir Propietario');

        // Simular la interacción del usuario - solo campos obligatorios
        fireEvent.change(nameInput, { target: { value: 'Peter Pan' } });
        fireEvent.change(addressInput, { target: { value: 'Neverland' } });

        // Simular el clic en el botón
        await act(async () => {
            fireEvent.click(createButton);
        });

        await waitFor(() => {
            expect(axios.post).toHaveBeenCalledTimes(1);
        }, { timeout: 3000 });
    });

    test('should handle API errors gracefully when fetching owners', async () => {
        // Mockear el error de forma más específica
        const networkError = new Error('Network Error');
        axios.get.mockRejectedValue(networkError);

        render(<OwnerManagement />);

        // Verificar que no se muestran owners cuando hay error
        await waitFor(() => {
            expect(screen.queryByText('John Doe')).not.toBeInTheDocument();
            expect(screen.queryByText('Jane Smith')).not.toBeInTheDocument();
        }, { timeout: 3000 });

        // Dar tiempo a que se ejecute console.error
        await new Promise(resolve => setTimeout(resolve, 100));

        // Verificar que se llamó a console.error
        expect(console.error).toHaveBeenCalledWith('Error al obtener los propietarios:', networkError);
    });

    test('should display edit modal when edit button is clicked', async () => {
        const mockOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
        ];
        axios.get.mockResolvedValue({ data: mockOwners });

        render(<OwnerManagement />);

        // Esperar a que se cargue el owner
        await screen.findByText('John Doe');

        // Buscar y hacer clic en el botón de editar
        const editButtons = screen.getAllByText('Editar');
        await act(async () => {
            fireEvent.click(editButtons[0]);
        });

        // Verificar que el modal de edición se muestra
        await waitFor(() => {
            expect(screen.getByText('Editar Propietario')).toBeInTheDocument();
        }, { timeout: 3000 });
    });

    test('should display delete confirmation modal when delete button is clicked', async () => {
        const mockOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
        ];
        axios.get.mockResolvedValue({ data: mockOwners });

        render(<OwnerManagement />);

        // Esperar a que se cargue el owner
        await screen.findByText('John Doe');

        // Buscar y hacer clic en el botón de eliminar
        const deleteButtons = screen.getAllByText('Eliminar');
        await act(async () => {
            fireEvent.click(deleteButtons[0]);
        });

        // Verificar que el modal de confirmación se muestra
        await waitFor(() => {
            expect(screen.getByText('Confirmar Eliminación')).toBeInTheDocument();
        }, { timeout: 3000 });
    });

    test('should handle server validation errors', async () => {
        const mockOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
        ];
        axios.get.mockResolvedValue({ data: mockOwners });

        // Mockear error de validación del servidor
        const validationError = {
            response: {
                data: {
                    errors: {
                        Name: ['El nombre debe tener al menos 3 caracteres'],
                        Address: ['La dirección es obligatoria']
                    }
                }
            }
        };
        axios.post.mockRejectedValue(validationError);

        render(<OwnerManagement />);

        // Esperar a que se cargue el owner
        await screen.findByText('John Doe');

        // Llenar el formulario con datos que causarán error en el servidor
        const nameInput = screen.getByPlaceholderText(/Juan Pérez García/i);
        const addressInput = screen.getByPlaceholderText(/Avenida Principal 123, Ciudad/i);
        const createButton = screen.getByText('Añadir Propietario');

        // Datos que probablemente fallen la validación del servidor
        fireEvent.change(nameInput, { target: { value: 'Ab' } }); // Muy corto
        fireEvent.change(addressInput, { target: { value: 'Test' } }); // Dirección válida

        await act(async () => {
            fireEvent.click(createButton);
        });

        // Verificar que se hizo la llamada POST (debería fallar en el servidor)
        await waitFor(() => {
            expect(axios.post).toHaveBeenCalledTimes(1);
        }, { timeout: 3000 });
    });

    test('should cancel edit when cancel button is clicked', async () => {
        const mockOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
        ];
        axios.get.mockResolvedValue({ data: mockOwners });

        render(<OwnerManagement />);

        // Esperar a que se cargue el owner
        await screen.findByText('John Doe');

        // Abrir modal de edición
        const editButtons = screen.getAllByText('Editar');
        await act(async () => {
            fireEvent.click(editButtons[0]);
        });

        // Esperar a que el modal se abra
        await screen.findByText('Editar Propietario');

        // Buscar y hacer clic en el botón de cancelar
        const cancelButton = screen.getByText('Cancelar');
        await act(async () => {
            fireEvent.click(cancelButton);
        });

        // Verificar que el modal se cerró
        await waitFor(() => {
            expect(screen.queryByText('Editar Propietario')).not.toBeInTheDocument();
        }, { timeout: 3000 });
    });

    test('should handle successful owner creation', async () => {
        const mockOwners = [
            { idOwner: '1', name: 'John Doe', address: '123 Main St', birthday: '1980-01-01', photo: '/images/owner1.jpg' },
        ];

        axios.get.mockResolvedValueOnce({ data: mockOwners });
        axios.post.mockResolvedValue({
            data: { idOwner: '2', name: 'Test Owner', address: 'Test Address' }
        });

        render(<OwnerManagement />);

        // Esperar a que se carguen los datos iniciales
        await screen.findByText('John Doe');

        // Llenar formulario
        const nameInput = screen.getByPlaceholderText(/Juan Pérez García/i);
        const addressInput = screen.getByPlaceholderText(/Avenida Principal 123, Ciudad/i);
        const createButton = screen.getByText('Añadir Propietario');

        fireEvent.change(nameInput, { target: { value: 'Test Owner' } });
        fireEvent.change(addressInput, { target: { value: 'Test Address' } });

        await act(async () => {
            fireEvent.click(createButton);
        });

        // Verificar que se hizo la llamada POST
        await waitFor(() => {
            expect(axios.post).toHaveBeenCalledTimes(1);
        }, { timeout: 3000 });
    });
});
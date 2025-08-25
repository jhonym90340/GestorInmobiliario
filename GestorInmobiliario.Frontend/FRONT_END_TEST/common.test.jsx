import React from 'react';
import { render, screen } from '@testing-library/react';
import { Button, Input, Select, Card } from '../src/components/common.jsx';

describe('Common UI Components', () => {
    // Prueba para el componente Button
    test('Button should render with children', () => {
        render(<Button>Click Me</Button>);
        const buttonElement = screen.getByRole('button', { name: /click me/i });
        expect(buttonElement).toBeInTheDocument();
    });

    // Prueba para el componente Input
    test('Input should render', () => {
        render(<Input placeholder="Enter text" />);
        const inputElement = screen.getByPlaceholderText(/enter text/i);
        expect(inputElement).toBeInTheDocument();
    });

    // Prueba para el componente Select
    test('Select should render with options', () => {
        render(<Select><option>Option 1</option></Select>);
        const selectElement = screen.getByRole('combobox');
        expect(selectElement).toBeInTheDocument();
        expect(screen.getByText('Option 1')).toBeInTheDocument();
    });

    // Prueba para el componente Card
    test('Card should render with children', () => {
        render(<Card>
            <h3>Test Card</h3>
            <p>This is a test card content.</p>
        </Card>);
        const cardTitle = screen.getByText('Test Card');
        const cardContent = screen.getByText('This is a test card content.');
        expect(cardTitle).toBeInTheDocument();
        expect(cardContent).toBeInTheDocument();
    });
});
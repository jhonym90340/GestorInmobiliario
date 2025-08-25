import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import ImageWithFallback from '../src/components/ImageWithFallback';

describe('ImageWithFallback Component', () => {
    // Prueba para renderizar con una imagen válida
    test('should render the image with valid src', () => {
        render(<ImageWithFallback src="/images/test.jpg" alt="Test Image" />);
        const image = screen.getByRole('img', { name: /test image/i });
        expect(image).toBeInTheDocument();
        expect(image).toHaveAttribute('src', 'http://localhost:50000/images/test.jpg');
    });

    // Prueba para mostrar la imagen de reserva si la imagen principal falla
    test('should display fallback image on error', () => {
        render(<ImageWithFallback src="/non-existent-image.jpg" alt="Fallback Image" />);
        const image = screen.getByRole('img', { name: /fallback image/i });

        // Simula un evento de error de carga de imagen
        fireEvent.error(image);

        // Verifica que la URL de la imagen sea la de reserva
        expect(image.src).toContain("placehold.co");
    });

    // Prueba para manejar URLs completas
    test('should handle full URLs correctly', () => {
        const fullUrl = 'http://example.com/image.png';
        render(<ImageWithFallback src={fullUrl} alt="Full URL Image" />);
        const image = screen.getByRole('img', { name: /full url image/i });
        expect(image).toHaveAttribute('src', fullUrl);
    });
});
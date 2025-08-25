// jest.config.js
export default {
  testEnvironment: 'jsdom',
  transform: {
    '^.+\\.(js|jsx)$': 'babel-jest',
  },
  transformIgnorePatterns: [
    '/node_modules/(?!(axios|prop-types|react-router-dom|react-dom)/)',
  ],
moduleNameMapper: {
  '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
  '\\.(gif|ttf|eot|svg|png|jpeg|jpg)$': '<rootDir>/__mocks__/fileMock.js',
  '^@/(.*)$': '<rootDir>/src/$1', // <-- Esta es la corrección
},
 setupFilesAfterEnv: ['<rootDir>/jest.setup.js', '@testing-library/jest-dom'],
  moduleFileExtensions: ['js', 'jsx', 'json', 'node'],
  testMatch: [
    '**/FRONT_END_TEST/**/*.test.js',
    '**/FRONT_END_TEST/**/*.test.jsx',
  ],
};
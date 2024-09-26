/** @type {import('ts-jest').JestConfigWithTsJest} */
module.exports = {
    preset: 'ts-jest',
    testEnvironment: 'node',
    verbose: true,
    testPathIgnorePatterns: ['<rootDir>/node_modules/', '<rootDir>/dist/'],
    collectCoverage: true,
    collectCoverageFrom: ['src/**/*.ts', '__data__/**/*.ts'],
    coverageReporters: ['text'],
    coverageThreshold: {
        global: {
            branches: 95,
            functions: 95,
            lines: 95,
            statements: 95
        }
    },
    testMatch: ['<rootDir>/__tests__/**/*.test.ts'],
    setupFilesAfterEnv: ['jest-extended/all']
};

import globals from 'globals';
import pluginJs from '@eslint/js';
import tseslint from 'typescript-eslint';

export default [
    { languageOptions: { globals: globals.browser } },
    pluginJs.configs.recommended,
    ...tseslint.configs.recommended,
    ...tseslint.configs.stylistic,
    {
        rules: {
            // If I want the openness of an interface instead of a type for object definitions,
            // then I'll make the thing an interface. Otherwise this recommendation is silly IMHO.
            // Make it a warning, so it's still visible just in case; but that it doesn't block CI.
            '@typescript-eslint/consistent-type-definitions': 'warn',

            // Sometimes it's good practice to have placeholder variables that are then unused,
            // such as in mock functions, parameter ordering, etc. Ensu
            '@typescript-eslint/no-unused-vars': [
                'error',
                {
                    argsIgnorePattern: '^_[^_].*$|^_$',
                    varsIgnorePattern: '^_[^_].*$|^_$',
                    caughtErrorsIgnorePattern: '^_[^_].*$|^_$'
                }
            ]
        }
    }
];

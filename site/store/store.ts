import { configureStore } from '@reduxjs/toolkit';
import objectsReducer from './slices/objects-slice';

const Store = configureStore({
    reducer: {
        objects: objectsReducer,
    },
});

export type RootState = ReturnType<typeof Store.getState>;
export type AppDispatch = typeof Store.dispatch;
export default Store;

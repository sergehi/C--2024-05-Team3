import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface ObjectType {
    id: string;
    [key: string]: any;
}

interface ObjectsState {
    [key: string]: ObjectType[];
}

const initialState: ObjectsState = {};

const objectsSlice = createSlice({
    name: "objects",
    initialState,
    reducers: {
        addOrUpdateObject: (
            state,
            action: PayloadAction<{ type: string; object: ObjectType }>
        ) => {
            const { type, object } = action.payload;
            const list = state[type] || [];
            const existingIndex = list.findIndex((item) => item.id === object.id);

            if (existingIndex !== -1) {
                // Обновляем существующий объект
                list[existingIndex] = { ...list[existingIndex], ...object };
            } else {
                // Добавляем новый объект
                list.push(object);
            }

            state[type] = list;
        },
        removeObjectById: (
            state,
            action: PayloadAction<{ type: string; id: string }>
        ) => {
            const { type, id } = action.payload;
            state[type] = (state[type] || []).filter((item) => item.id !== id);
        },
    },
});

export const { addOrUpdateObject, removeObjectById } = objectsSlice.actions;
export default objectsSlice.reducer;

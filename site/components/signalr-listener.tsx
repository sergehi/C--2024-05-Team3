"use client";

import { useEffect } from "react";
import { connection, startConnection } from "@/server/signalr-connection";
import { useDispatch } from "react-redux";
import { addOrUpdateObject, removeObjectById } from "@/store/slices/objects-slice";

const SignalRListener = ({ children }: { children: React.ReactNode }) => {
    const dispatch = useDispatch();

    useEffect(() => {
        startConnection();

        const handleEvent = (event: string, data: any) => {
            switch (event) {
                case "ReceiveUpdate":
                case "ReceiveCreate":
                    dispatch(addOrUpdateObject({ type: data.objectType, object: data }));
                    break;
                case "ReceiveDelete":
                    dispatch(removeObjectById({ type: data.objectType, id: data.id }));
                    break;
                default:
                    console.warn(`Неизвестный тип события: ${event}`);
            }
        };

        ["ReceiveUpdate", "ReceiveCreate", "ReceiveDelete"].forEach((event) => {
            connection.on(event, (data) => handleEvent(event, data));
        });

        return () => {
            connection.stop();
        };
    }, [dispatch]);

    return <>{children}</>;
};

export default SignalRListener;

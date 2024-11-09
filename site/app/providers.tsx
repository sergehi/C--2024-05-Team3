"use client";

import * as React from "react";
import { NextUIProvider } from "@nextui-org/system";
import { useRouter } from "next/navigation";
import { ThemeProvider as NextThemesProvider } from "next-themes";
import { ThemeProviderProps } from "next-themes";
import { Provider } from "react-redux";
import Store from "@/store/store";
import { SessionProvider } from "next-auth/react";
import SignalRListener from "@/components/signalr-listener";

export interface ProvidersProps {
  children: React.ReactNode;
  themeProps?: ThemeProviderProps;
}

export function Providers({ children, themeProps }: ProvidersProps) {
  const router = useRouter();

  return (
    <SessionProvider>
      <Provider store={Store}>
        <NextUIProvider navigate={router.push}>
          <NextThemesProvider {...themeProps}>
            <SignalRListener>
              {children}
            </SignalRListener>
          </NextThemesProvider>
        </NextUIProvider>
      </Provider>
    </SessionProvider>
  );
}

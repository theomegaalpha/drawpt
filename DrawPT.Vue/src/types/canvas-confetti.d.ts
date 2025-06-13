declare module 'canvas-confetti' {
  interface ConfettiOptions {
    particleCount?: number;
    angle?: number;
    spread?: number;
    startVelocity?: number;
    decay?: number;
    gravity?: number;
    drift?: number;
    ticks?: number;
    origin?: {
      x?: number;
      y?: number;
    };
    colors?: string[];
    shapes?: string[];
    zIndex?: number;
    disableForReducedMotion?: boolean;
    scalar?: number;
    [key: string]: any; // Allow other properties
  }

  interface ConfettiInstance {
    (options?: ConfettiOptions): Promise<null> | null;
    reset(): void;
  }

  const confetti: ConfettiInstance;
  export default confetti;
}

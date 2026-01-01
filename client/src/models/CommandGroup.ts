import type { DrawCommand } from "./DrawCommand"

export type CommandGroup = {
  prompt: string
  commands: DrawCommand[]
}
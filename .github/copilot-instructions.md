## .github/copilot-instructions.md

### Mod Overview and Purpose
The mod, referred to here as "VXMASSE", is a seasonal modification for RimWorld that centers around adding festive seasonal content to the player's colony. It incorporates celebratory elements such as parties and presents which are aimed at enhancing the social and recreational aspects of gameplay.

### Key Features and Systems
- **Festive Parties**: Introduces joinable community parties that colonists can voluntarily participate in. These events are managed through specialized job classes and lord job toil systems.
- **Gift Mechanics**: Introduces a new building type, a "Present", which can store items. Players can pack and unpack gifts, enhancing the interactive gameplay experience.
- **Special Incidents**: Adds incidents related to the festive theme, where presents are dropped at specified map locations.

### Coding Patterns and Conventions
- **Conventions**: Follows typical C# style for class and method naming, using PascalCase for class names and method names.
- **Inheritance**: Utilizes inheritance to extend functionality of base game classes, allowing for customized behaviors (e.g., `JobGiver_Sit` extends `JobGiver_Wander`).
- **Modifiers**: Uses access modifiers effectively to encapsulate class behavior and ensure secure manipulation of game state (e.g., `private` methods in `LordJob_Joinable_CParty`).

### XML Integration
- **Defining New Defs**: New definitions (Defs) for duties, buildings, and incidents are likely required. This integration is not detailed but is crucial for creating new game content and features.
- **DefOf Classes**: Ensures that all new definitions are referenced in `DefOf` classes (`VDutyDefOf`, `XDefOf`) to facilitate easy and consistent access across code.

### Harmony Patching
- Although not specifically mentioned in the class summaries above, Harmony is often used in RimWorld modding to modify game behavior without altering base game files. Ensure any detouring or postfixes are clearly documented to maintain compatibility and upgrade paths.

### Suggestions for Copilot
- **File Naming**: Suggest naming conventions for new classes and files that reflect their purpose (e.g., `GiftDispenser` for classes related to gifting mechanisms).
- **Method Stubs**: Provide method stubs with summaries for new functionality, such as expanding `EjectContents` to include event triggers.
- **Error Handling**: Incorporate error handling scaffolds around methods interacting with the game world to ensure robustness.
- **Testing Utilities**: Create test routines that can simulate parties and incidents without full game integration for development efficiency.
- **XML Snippets**: Generate XML snippets for new Defs that can be directly modified and included in the `Defs` directory of the mod.

This detailed guidance serves as a comprehensive overview for implementing, expanding, and maintaining the VXMASSE mod with assistance from GitHub Copilot. It emphasizes modular design, high compatibility, and ease of integration with RimWorld's existing modding framework.

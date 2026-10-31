# Vanilla Christmas Expanded (Continued) GitHub Copilot Instructions

## Mod Overview and Purpose
**Vanilla Christmas Expanded (Continued)** is a festive mini-mod for RimWorld that adds various Christmas-themed mechanics to enhance your colony's celebration. The mod introduces a Christmas tree, a fireplace, and other festive elements, such as eggnog and a new event where players can receive presents.

## Key Features and Systems
- **Christmas Tree**: A 1x1 structure that beautifies rooms and allows for a "Presents!" event.
- **Fireplace**: Adds a festive atmosphere to your colony.
- **Eggnog**: A new consumable item to enjoy during the festive season.
- **Presents Event**: Allows players to receive gifts and experience a festive gathering.
- **French Translation**: Provides support for French-speaking players.
- **Simple FX: Smoke**: Adds a visual effect when using certain items.
- **Gift Drop Option**: Allows players to disable gift-drops if desired.

## Coding Patterns and Conventions
- **C# Classes**: Follow standard RimWorld conventions for creating new jobs, lords, and settings.
- **XML Definitions**: Use the same structure and naming conventions as vanilla RimWorld definitions.
- **Modding Best Practices**: Adhere to best practices for modding RimWorld, such as using Harmony for patching and maintaining compatibility with other mods.

## XML Integration
XML files in this mod define new game elements and configurations. Key files include:
- **DutyDef**: Defines new duties related to the Christmas event.
- **ThoughtDef**: Adds new thoughts associated with the festive season.
- **DesignatorDropdownGroupDef**: Organizes designators for easier access.
- **IncidentDef**: Defines the "Presents Drop" event.
- **ThingDef**: Describes the new Christmas tree and fireplace structures.

### Example XML: ThingDef
xml
<ThingDef>
    <defName>ChristmasTree</defName>
    <label>Christmas Tree</label>
    <description>A beautiful Christmas tree that can be placed in any room.</description>
    <graphicData>
        <texPath>Things/Buildings/ChristmasTree</texPath>
    </graphicData>
    <statBases>
        <Beauty>5</Beauty>
    </statBases>
    <thingCategories>
        <li>Building</li>
    </thingCategories>
    <building>
        <isEdifice>true</isEdifice>
    </building>
</ThingDef>


## Harmony Patching
Harmony is used to patch existing RimWorld code to add new functionality without altering the original codebase. Key patches include:
- **JobGiver_Sit**: Custom job giver for sitting during festive gatherings.
- **LordJob_Joinable_CParty**: Custom lord job for managing Christmas parties.
- **LordToil_CParty**: Custom lord toil for updating duties during parties.
- **MapComponentXmass**: Custom map component for managing Christmas-related events.

### Example Harmony Patch: JobGiver_Sit
csharp
using HarmonyLib;
using RimWorld;
using Verse;

[HarmonyPatch(typeof(JobGiver_Sit))]
[HarmonyPatch("GetExactWanderDest", MethodType.Instance)]
public class Patch_JobGiver_Sit
{
    public static void Postfix(ref IntVec3 __result, Pawn pawn)
    {
        if (pawn.Map.mapComp_Xmass.IsChristmasEventActive)
        {
            __result = IntVec3.Invalid;
        }
    }
}


## Suggestions for Copilot
- **Code Snippets**: Provide snippets for creating new jobs, lords, and settings.
- **XML Templates**: Offer templates for defining new game elements in XML.
- **Harmony Examples**: Share examples of Harmony patches for modifying existing RimWorld functionality.
- **Debugging Tips**: Suggest best practices for debugging and maintaining compatibility with other mods.
- **Localization**: Assist with adding support for additional languages.

## Contribution Guidelines
- **Fork the Repository**: Clone the repository to your local machine.
- **Create a Branch**: Create a new branch for your changes.
- **Make Changes**: Add, update, or fix code and XML files.
- **Commit Changes**: Commit your changes with a clear and concise message.
- **Push Changes**: Push your changes to your forked repository.
- **Create a Pull Request**: Submit a pull request to the main repository.

## Support
For any issues or questions, please use the Discord channel for error reporting. Do not create discussion threads, as the author may not receive notifications. If you have a solution to a problem, please post it to the GitHub repository.

## License
Graphics and code are created by Oskar Potocki and Kikohi. This mod is licensed under the CC BY-NC-ND 4.0 license.


This detailed .github/copilot-instructions.md file provides a comprehensive guide for contributors to the Vanilla Christmas Expanded (Continued) mod, covering the mod's purpose, key features, coding conventions, XML integration, and Harmony patching. It also includes suggestions for GitHub Copilot and contribution guidelines to ensure a smooth development process.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.


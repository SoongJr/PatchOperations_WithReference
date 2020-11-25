# PatchOperations with references
Adds clones of the vanilla PatchOperations Insert, Add and Replace with an added feature: the `<value>` node may contain any number of `<xpath>` nodes which will be evaluated and the result will replace the `<xpath>` node.  

## Examples
This Patch replaces the defs that require one drape in bedrooms and two drapes in throne rooms of royal pawns. Without the ability to reference the `<count>`, this would have to be two separate operations with hardcoded values. And it would not work when Ludeon decides that two drapes aren't enough!  
The additional xpath to dynamically reference the `thingDef` just reduces redundancy and shows that you can also reference text-nodes (the content of nodes) when you need to change the name of the node.
```
  <Operation Class="Soong.PatchOperationReplace_WithReference">
    <xpath>
      /Defs/RoyalTitleDef/throneRoomRequirements/li[@Class = "RoomRequirement_ThingCount"][thingDef = "Drape"]
      | /Defs/RoyalTitleDef/bedroomRequirements/li[@Class = "RoomRequirement_ThingCount"][thingDef = "Drape"]
    </xpath>
    <value>
      <li Class="Dwarves.RoomRequirement_ThingCountAnyOf">
        <things>
          <li><xpath>./thingDef/text()</xpath></li>
        </things>
        <xpath>count</xpath>
      </li>
    </value>
  </li>
```

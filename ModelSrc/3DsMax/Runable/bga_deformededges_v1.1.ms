/*//////////   SCRIPT:   Deformed Edges v1.0   //////////*/	
/*//////////   WRITTEN BY:   Branko Zivkovic   //////////*/
/*//////////   EMAIL:   barigazy@hotmail.com   //////////*/
/*
Description:
With this tool user can deform selected edges of Editable Poly objects.
This will give your model a more natural look with raw edge.
For example: wooden or concrete pillars or walls, pieces of clay or ice, rude metal etc.
Follow these five step:
1. Select your model (the conditions are: model must be converted as Editable Poly object with none modifiers assigned)
2. Manually select edges or you can use the button *Select Contour Edges* which will select all edges 
    with the given minimum and maximum angle between two faces who share same edge.
3. When the edges are selected press *Analize Topology* button to recive some inportant 
    pararametars for precalculation process.
4. I suggest to use default settings for the first deformation process by pressind *Defor Selected Edges*  Button. Then activate *Interactive mode*
   which will allow you to have direct feedback of any changes on your model.
5. You can also use *Peview UNS* (use NURMS Subdivision) with display iterations, 
    or remove all changes on model topology (*Revert Object Topology*)
	
N O T E: If your object has a large polygon count (high poly model) then do not recommend that you use*Interactive mode*
or if you do choose to use this mode in the previously described conditions then try to increase or decrease spinner value only by one.

Installation:
Place the bga_DeformedEdges_v1.0.ms file into a max script directory.

Credits: 
Denis Trofimov (Denis T) for his great input regarding optimization of code.

History:
Version 1.1 New Features:
- now you can use BEVEL operation to create a stitches
NOTE: To disable bevel option just set *Bevel Height* value to ZERO.
- added Preview Checker Map options for cheking the model UV's.

About script *Deformed Edges *
This scripts pack is free and opensource. It can be used for both personal and professional work.
Use at your own risk. Author (me) can not be held responsible 
for any loss of earnings or data this tool may cause.
*/

global deRoll
if deRoll  != undefined do try (destroyDialog deRoll) catch()
rollout deRoll " Deformed Edges v1.1"
(
	global backupObj = undefined
	local defObj, polyCount, ctrlState, objMtl
	local divisionsArr = #(), selEdges = #{}, freezeSpn = false, spnValArr = #()
	fn filtObj obj = (isKindOf obj Editable_Poly and obj.modifiers.count == 0)
	fn edgeFacesAngle dir1 dir2 = (acos (dot (normalize dir2) (normalize dir1)))
	fn getSnapshot obj &backupObj =
	(
		backupObj = (p = copy obj).baseobject
		delete p
	)
	fn checkerMtl clr1: clr2: tiling: =
	(
		local deCM = Checker name:"deCheckMap" color1:clr1 color2:clr2
		deCM.coords.u_tiling = deCM.coords.v_tiling = tiling
		deMtl = Standardmaterial name:"deMaterial" diffuseMap:deCM showInViewport:true
	)
	fn selectContourEdges obj minAngle maxAngle =
	(
		local edgelist = #{}
		getedgefaces = polyOp.getEdgeFaces
		getFnormal = polyop.getFaceNormal
		edgeCnt = polyop.getNumEdges obj
		for ed in 1 to edgeCnt where (getedgefaces obj ed).count == 2 do
		(
			facesArr = (getedgefaces obj ed)
			dir1 = (getFnormal obj facesArr[1])
			dir2 = (getFnormal obj facesArr[2])
			theAngle = edgeFacesAngle dir1 dir2
			if theAngle >= minAngle and theAngle <= maxAngle do append edgelist ed
		)
		if edgelist.numberSet != 0 do
		(
			obj.selectededges = edgelist
			if getCommandPanelTaskMode() != #modify do setCommandPanelTaskMode mode:#modify
			modPanel.setCurrentObject obj	; subobjectLevel = 2
		)
	)
	fn calculateDivisions obj edgeslist divVal =
	(
		getedgeverts = polyOp.getEdgeVerts
		getvertpos = polyOp.getVert
		edgeLenghts = for edge in edgeslist collect
		(
			vv = getedgeverts obj edge
			distance (getvertpos obj vv[1]) (getvertpos obj vv[2])
		)
		maxEdgesLength = amax edgeLenghts
		for eLength in edgeLenghts collect (floor (eLength / (maxEdgesLength/divVal))) as integer
	)
	fn deformEdges obj edgelist divideArr polyCnt bevelH: bevelO: chamferA: rand: seedV: =
	(
		local verts = #{}
		seed seedV ; id = 0
		obj.edgeChamferSegments = 1
		chamferedges = polyOp.chamferEdges
		getedgeverts = polyOp.getEdgeVerts
		movevert = polyOp.moveVert
		divideedge = polyOp.divideEdge
		getnumedges = polyOp.getNumEdges
		getvertpos = polyOp.getVert
		for edge in edgelist do
		(
			divideIdx = divideArr[id += 1]
			if divideIdx != 0 do (for d = divideIdx to 1 by -1.0 do divideedge obj edge (1 - 1/(d+1)))
	   )	
		chamferedges obj obj.selectededges chamferA
	   if bevelH != 0 then 
	   (
		   obj.bevelType = 1
		   newNumFaces = polyOp.getNumFaces obj
		   newFaces = #{(polyCnt+1)..newNumFaces}
		   polyop.bevelFaces obj newFaces bevelH bevelO
			verts = polyOp.getVertsUsingFace obj obj.selectedfaces
	   )
	   else (verts = polyOp.getVertsUsingEdge obj obj.selectededges)
		with redraw off for v in verts do movevert obj v (random -rand rand)
		polyOp.retriangulate obj obj.faces
		polyOp.setFaceSmoothGroup obj obj.faces 0
	)
	label deLbl0 " ••• Pick and Prepare Object •••" pos:[0,0] width:160 height:16 style_sunkenedge:true
	pickbutton pbtn_PickEPoly "Pick E-Poly Object" pos:[2,18] width:136 height:18 tooltip:"Pick Editable Poly Object" filter:filtObj
	button btn_Clean "C" pos:[140,18] width:18 height:18 tooltip:"Clear"
	button btn_ContEdg "Select Contour Edges" pos:[2,38] width:136 height:18 tooltip:"Pick Editable Poly Object"
	button btn_Remove "R" pos:[140,38] width:18 height:18 tooltip:"Reset Angle Spinners"
	spinner spn_minAng "Min Angle:           " pos:[3,58] fieldwidth:60 height:16 range:[-360,360,60] type:#float
	spinner spn_maxAng "Max Angle:          " pos:[2,76] fieldwidth:60 height:16 range:[-360,360,120] type:#float
	label deLbl1 "        ••• Deform Options ••• " pos:[0,94] width:160 height:16 style_sunkenedge:true
	button btn_CheckTopo "Click to Analize Topology" pos:[2,112] width:156 height:18 tooltip:"Pick Editable Poly Object"
	label deLbl2 " > object need to be analized!" pos:[0,132] width:160 height:16
	spinner spn_bevelH "Bevel Height:       " pos:[2,150] fieldwidth:60 height:16 range:[-1e3,1e3,0] type:#float
	spinner spn_bevelO "Bevel Outline:      " pos:[2,168] fieldwidth:60 height:16 range:[-1e3,1e3,-0.2] type:#float
	spinner spn_chamfA "Chamfer Amount:" pos:[2,186] fieldwidth:60 height:16 range:[0.001,1e3,1] type:#float
	spinner spn_edgeDiv "Edge Division:      " pos:[2,204] fieldwidth:60 height:16 range:[1,10,3] type:#integer
	spinner spn_vertOff "Vertex Offset:     " pos:[2,222] fieldwidth:60 height:16 range:[0,1e4,0.1] type:#float
	button btn_rndSeed "Random Seed" pos:[2,240] width:85 height:16 tooltip:"Pick Editable Poly Object"
	spinner spn_Seed "" pos:[87,240] fieldwidth:60 height:16 range:[0e6,9e6,123456] type:#integer
	checkbutton cbtn_PrevUNS "Preview UNS" pos:[2,258] width:85 height:16 highlightColor:[28,28,28] tooltip:"Use NURMS Sudivision"
	spinner spn_UNS "" pos:[87,258] fieldwidth:60 height:16 range:[0,5,2] type:#integer tooltip:"Display Iterations"
	checkbutton cbtn_PrevCM "Preview CM" pos:[2,276] width:61 height:16 highlightColor:[28,28,28] tooltip:"Use Checker Map"
	spinner spn_Tile "" pos:[87,276] fieldwidth:60 height:16 range:[0,1e3,5] type:#integer tooltip:"Checker Map Tiling"
	colorpicker cp_clr1 "" pos:[61,276] fieldwidth:10 height:16 color:[0,0,0] modal:false
	colorpicker cp_clr2 "" pos:[73,276] fieldwidth:10 height:16 color:[28,89,177] modal:false
	checkbutton cbtn_InterMode "Use Interactive Mode" pos:[2,294] width:156 height:18 highlightColor:[28,28,28] enabled:false
	button btn_RevertTopo "Revert Object Topology" pos:[2,314] width:156 height:18 enabled:false
	button btn_Deform "Deform Selected Edges" pos:[2,334] width:156 height:18 enabled:false
	timer clock "Reseter" interval:10 active:false
	local spnCtrls = #(spn_bevelH, spn_bevelO, spn_chamfA, spn_edgeDiv, spn_vertOff, spn_Seed)
	fn resetAllCtrls =
	(
		pbtn_PickEPoly.text = "Pick E-Poly Object"
		deLbl2.text = " > object need to be analized!"
		spn_minAng.value = 60
		spn_maxAng.value = 120
		spn_bevelH.value = 0
		spn_bevelO.value = -0.2	
		spn_chamfA.value = 1
		spn_edgeDiv.value = 3
		spn_vertOff.value = 0.1
		spn_Seed.value = 123456
		spn_UNS.value = 2
		spn_Tile.value = 5
		cbtn_PrevUNS.checked = cbtn_PrevCM.checked = cbtn_InterMode.checked = false
		cbtn_InterMode.enabled = btn_RevertTopo.enabled = btn_Deform.enabled = false
		defObj = undefined ; backupObj = undefined
		free selEdges ; free divisionsArr ; gc()
	)
	on pbtn_PickEPoly picked obj do
	(
		if isValidNode obj == true do
		(
			pbtn_PickEPoly.text = obj.name
			getSnapshot obj &backupObj
			defObj = obj
			clock.active = true
		)
	)
	on btn_Clean pressed do (resetAllCtrls())
	on btn_ContEdg pressed do
	(
		t1 = timestamp() ; m1 = heapfree
		if isValidNode defObj do (selectContourEdges defObj spn_minAng.value spn_maxAng.value)
		format "time:% memory:%\n" (timestamp() - t1) (m1 - heapfree)
	)
	on btn_Remove pressed do (spn_minAng.value = 60 ; spn_maxAng.value = 120)
	on btn_CheckTopo pressed do
	(
		if isValidNode defObj do
		(
			selEdges = defObj.selectededges as bitarray
			if selEdges.numberSet == 0 then (#abort ; messageBox "Select edges first!" title:"Warning" beep:false) else
			(
				btn_Deform.enabled = true
				getSnapshot defObj &backupObj
				deLbl2.text = " > ready for deformation!"
				divisionsArr = calculateDivisions defObj selEdges spn_edgeDiv.value
				polyCount = polyOp.getNumFaces defObj
				if polyCount >= 1e4 do (messageBox ("Object polygon count is "+polyCount as string+"\r\nNot recommended to use interactive mode.") title:"Warning" beep:false)
			)
		)
	)
	on spn_bevelH changed val do
	(
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do 
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:val bevelO:spn_bevelO.value chamferA:spn_chamfA.value rand:offset seedV:spn_Seed.value
		)
	)
	on spn_bevelO changed val do
	(
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do 
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:val chamferA:spn_chamfA.value rand:offset seedV:spn_Seed.value
		)
	)
	on spn_chamfA changed val do
	(
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do 
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:spn_bevelO.value chamferA:val rand:offset seedV:spn_Seed.value
		)
	)
	on spn_edgeDiv changed val do
	(
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			divisionsArr = calculateDivisions defObj selEdges val
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:spn_bevelO.value chamferA:spn_chamfA.value rand:offset seedV:spn_Seed.value
		)
	)
	on spn_vertOff changed val do
	(
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			offset = [val,val,val]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:spn_bevelO.value chamferA:spn_chamfA.value rand:offset seedV:spn_Seed.value
		)
	)
	on btn_rndSeed pressed do
	(
		spn_Seed.value = random 100000.0 999999
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:spn_bevelO.value chamferA:spn_chamfA.value rand:offset seedV:spn_Seed.value
		)
	)
	on spn_Seed changed val do
	(
		if cbtn_InterMode.checked == true do
		(
			if polyOp.getNumEdges defObj != polyCount do
			(
				defObj.baseobject = backupObj
				getSnapshot defObj &backupObj
				defObj.selectededges = selEdges
			)
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:spn_bevelO.value chamferA:spn_chamfA.value rand:offset seedV:val
		)
	)
	on cbtn_PrevUNS changed state do
	(
		if isValidNode defObj then
		(
			local ctrlArr = #(cbtn_InterMode,btn_RevertTopo,btn_Deform)
			if state == on then
			(
				defObj.surfSubDivide = on ; defObj.iterations = spn_UNS.value
				ctrlState = for c in ctrlArr collect (c.enabled) ; append ctrlState ctrlArr[1].checked
				for s in 1 to spnCtrls.count do (append spnValArr spnCtrls[s].value ; freezeSpn = true)
				cbtn_InterMode.enabled = cbtn_InterMode.checked = btn_RevertTopo.enabled = btn_Deform.enabled = false
			)
			else
			(
				defObj.surfSubDivide = off ; freezeSpn = false ; free spnValArr
				for i in 1 to 3 do (ctrlArr[i].enabled = ctrlState[i])
				ctrlArr[1].checked = ctrlState[4]
			)
		) else cbtn_PrevUNS.checked = false
	)
	on spn_UNS changed val do
	(
		if isValidNode defObj and cbtn_PrevUNS.checked == true do defObj.iterations = val
	)
	on cbtn_PrevCM changed state do
	(
		if isValidNode defObj then
		(
			if state == on then
			(
				objMtl = defObj.material
				defObj.material = checkerMtl clr1:cp_clr1.color clr2:cp_clr2.color tiling:spn_Tile.value
			)
			else (defObj.material = objMtl ; objMtl = undefined)
		) 
		else cbtn_PrevCM.checked = false
	)
	on spn_Tile changed val do
	(
		if isValidNode defObj and cbtn_PrevCM.checked == true and defObj.material.name == "deMaterial" do 
		(defObj.material.diffusemap.coords.u_tiling = defObj.material.diffusemap.coords.v_tiling = val)
	)
	on cp_clr1 changed clr do
	(
		if isValidNode defObj and cbtn_PrevCM.checked == true and defObj.material.name == "deMaterial" do defObj.material.diffusemap.color1 = clr
	)
	on cp_clr2 changed clr do
	(
		if isValidNode defObj and cbtn_PrevCM.checked == true and defObj.material.name == "deMaterial" do defObj.material.diffusemap.color2 = clr
	)	
	on btn_RevertTopo pressed do
	(
		if isValidNode defObj do
		(
			format "%\n" backupObj
			defObj.baseobject = backupObj
			getSnapshot defObj &backupObj
			defObj.selectededges = selEdges
			setCommandPanelTaskMode mode:#modify
			subobjectLevel = 2 ; btn_Deform.enabled = true
			btn_RevertTopo.enabled = cbtn_InterMode.enabled = cbtn_InterMode.checked = false
		)
	)
	on btn_Deform pressed do
	(
		if isValidNode defObj and selEdges.numberSet != 0 do
		(
			offset = [spn_vertOff.value,spn_vertOff.value,spn_vertOff.value]
			if getCommandPanelTaskMode() != #create do setCommandPanelTaskMode mode:#create
			t1 = timestamp() ; m1 = heapfree
			undo off deformEdges defObj selEdges divisionsArr polyCount bevelH:spn_bevelH.value bevelO:spn_bevelO.value chamferA:spn_chamfA.value rand:offset seedV:spn_Seed.value
			format "time:% memory:%\n" (timestamp() - t1) (m1 - heapfree)
			btn_RevertTopo.enabled = cbtn_InterMode.enabled = true
			btn_Deform.enabled = false
		)
	)
	on clock tick do 
	(
		if not isValidNode (getNodeByName pbtn_PickEPoly.text) do (resetAllCtrls() ; clock.active = false)
		if freezeSpn == true do (for v in 1 to 6 do spnCtrls[v].value = spnValArr[v])
	)
	on deRoll close do (backupObj = undefined ; gc() ; clearListener())
)
createdialog deRoll 160 354 10 110 style:#(#style_titlebar, #style_sysmenu, #style_toolwindow)
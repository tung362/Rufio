macroScript ChewEdges
category:"Bluesummers' Tools" 
tooltip:"Chews selected edges"
ButtonText: "Chew"
Icon:#("PolyTools",1)
silentErrors:true
(	
	-- A plug-in written by Alex Bluesummers
	-- Original concept by Aline Lima
	
	rollout Chewer "Chew"
	(
		local Magnitude, ChewIt		--<< Important functions.
		local ChewedLast = false	--<< Distinguish between 1st run.
		local lastFrequency			--<< Take note of the last frequency.
		
		-- Puttin' the name out there.
		hyperlink label1 "Mr. BlueSummers ©2008" \
			address:"http://www.mrbluesummers.com" align:#center
		
		group "Main"
		(
			spinner	Frequency	"Frequency:   "		type:#integer 	fieldwidth:36 	\
					tooltip: "Determines the number of connecting lines there are."	\
					range:[0,1E4,2] scale:.05
			spinner Depth		"Depth:   "			type:#float		fieldwidth:36 	\
					tooltip: "Determines the level of the chamfer."					\
					range:[0,1E4,(Magnitude ($.max - $.min))/100.0] scale:.05
			spinner Randomness	"Randomness:   "	type:#float		fieldwidth:36 	\
					tooltip: "Determines the severity of the randomness created."	\
					range:[0,1E4,(Magnitude ($.max - $.min))/100.0] scale:.05							
		)
		
		Button Okay		"Okay" 						across:2
		Button Cancel 	"Cancel"
		
		-- Frequency gets special UI Treatment because it changes on a small scale.
		-- If this Frequency isn't the same as the last frequency (by INT!) then update.
		on Frequency changed value do
			if value != lastFrequency do
				(lastFrequency = value; ChewIt())
		on Randomness changed value do	ChewIt()
		on Depth changed value do 		ChewIt()
		
		-- We don't really DISPLAY the effect so much as we just do it to the object,
		-- then undo it if the user decides against chewing or alters variables.
		-- Crude, I know.
		on Okay pressed do destroyDialog Chewer
		on Cancel pressed do (max undo; destroyDialog Chewer)
		
		-- On open, run the effect once to show the chewer at work.
		on Chewer open do 
		(
			ChewIt()
			ChewedLast = True
		)
		--		--		--
		-- SOME FUNCTIONS
		--		--		--
		fn magnitude v = return sqrt(v.x^2 + v.y^2 + v.z^2)
		
		fn ChewIt =
		(
			-- This is kind of a dirty way of handling the 1st time
			-- running of the macroscript, but whatever.  
			if ChewedLast do max undo		
			
			--		--		--		--		
			--	Chew the edges		--
			--		--		--		--
			local F_Normal_1, F_Normal_2
			local Faces
			local RandMask = #(#(),#())
			local EdgeArray, Vert
			
			undo "Chew" on
		    (	
				-- Grab all the edges selected so we may chew them.
				AllEdges = polyOp.getEdgeSelection $
				
				for i in AllEdges do
				(					
					-->> Proceed as normal <<
					-- Grab the edge we're currently working on.
					PolyOp.setEdgeSelection $ i
					
					-- Set chamfer level to "Depth" and chamfer.
			    	$.edgeChamfer = Depth.value
			    	$.EditablePoly.buttonOp #chamfer
					
					-- Connect the resulting segments "Frequency" times.
					$.connectEdgeSegments = Frequency.value
			    	$.EditablePoly.ConnectEdges ()
					
					-- Grab the resultant connections as an array.
					EdgeArray = (polyOp.getEdgeSelection $) as array
					
					-- Grab the randomness from the UI.
					Rand = Randomness.value

					-- Displace the vertices therein.
			    	for j = 1 to EdgeArray.count do
			    	(
						-- Grab verts from an edge in the array.
			    		TheseVerts = polyop.getEdgeVerts $ EdgeArray[j]				
	
						-- Make a random variable.
						RND = random -Rand Rand
			    		for k = 1 to 2 do
			    		(
							-- Grab one vertex.
			    			Vert = polyop.getVert $ TheseVerts[k]
							
							-- Offset the vertex by a portion of the randomization mask.
							PolyOp.setVert $ #{TheseVerts[k]} (Vert + RND)
			    		)						
			    	)
				)-- End "for each edge"
			)-- End Undo Block
		)-- End ChewIt function
	)-- End dialog def
	
	-->> When the plugin is started, check to make sure it's being used right.
	try
	(
		if $ == undefined										-- Nothing selected
			do throw "Nothing selected!"
		
		if ($.baseObject as string) != "Editable Poly" 			-- Not Edit Poly
			do throw "Object not an editable poly!"
			
		if ((polyOp.getEdgeSelection $) as array).count == 0	-- No edge selection.
			do throw "No edges selected in base object!"
		
		-->> Just in case, bring the user back to the modify panel, and to 
		-->> the edge selection.  Bring up the dialog.
		setCommandPanelTaskMode #modify
		modPanel.setCurrentObject $.baseObject
		setSelectionLevel $ #edge
		createDialog Chewer style:#(#style_boarder, #style_toolwindow) Modal:true
	)
	catch
	(
		messagebox ("An Error Occured"+ getCurrentException()) title:"Critical Error"
		destroydialog Chewer
	)
)-- End Macroscript

utility Chew "Chew It!"
(
	on Chew open do
	(
		closeUtility Chew
		macros.run "Bluesummers' Tools" "ChewEdges"
	)
)

# Start Level Button - Debug & Test Guide

## ✅ Button Status

The Start Level button has been configured with:
- **Target:** GameManager (LevelManager component)
- **Method:** BeginLevel()
- **Listener Type:** Persistent (survives scene reload)
- **Interactable:** True

---

## 🧪 Test Procedure

### Test 1: Verify Button Setup (In Editor)

1. Select the "Start Level Button" in hierarchy
   - Path: `Canvas/Store Root/Store Panel/Start Level Button`

2. In Inspector, scroll to **Button** component

3. Check **OnClick()** section:
   - Should show **1 event**
   - Target: **GameManager (LevelManager)**
   - Function: **LevelManager.BeginLevel**

### Test 2: Verify Button Click Detection

**Option A: Add Test Component**
1. Select Start Level Button
2. Add Component → **ButtonClickTest**
3. Enter Play Mode
4. Click the button
5. Console should show: `★★★ BUTTON CLICKED: Start Level Button ★★★`

**Option B: Check Console Logs**
1. Enter Play Mode
2. Click Start Level Button
3. Console should show (in order):
   ```
   BeginLevel() called!
   Store closed
   Store panel set to inactive
   Ammo refilled to 10
   Fortress spawned
   Level 1 started! levelInProgress = True
   ```

### Test 3: Visual Verification

1. Enter Play Mode
2. Store panel should be visible
3. Click "START LEVEL" button
4. **Expected Behavior:**
   - Store panel disappears
   - Cannon becomes active
   - Fortress is visible
   - HUD shows ammo count

---

## 🐛 Troubleshooting

### Symptom: Nothing happens when clicking button

**Diagnosis Steps:**

1. **Check Console First**
   - Open Console window (Window → General → Console)
   - Clear console (top-left button)
   - Click button
   - Look for ANY message

2. **If NO messages at all:**
   ```
   The button click isn't being registered
   ```
   **Solutions:**
   - Run **Tools → Fix EventSystem Input**
   - Verify EventSystem exists in scene
   - Check button has Image component
   - Verify button is on Canvas

3. **If you see "BeginLevel() called!" but nothing happens:**
   ```
   Button works, but BeginLevel has issues
   ```
   **Check the following logs:**
   - "Store closed" → StoreManager working
   - "Fortress spawned" → FortressSpawner working
   - "levelInProgress = True" → Level state correct

4. **If store doesn't close:**
   ```
   StoreManager.storePanel reference issue
   ```
   **Solution:**
   - Run **Tools → Fix Store UI Input**
   - Verify StoreManager.storePanel = "Store Root"

---

## 🔍 Debug Commands

### Re-connect Button (if needed)
```
Tools → Debug Start Button
```
This will:
- Find the button
- Check current listeners
- Clear old listeners
- Add fresh persistent listener
- Save changes

### Verify Complete Setup
```
Tools → Verify Complete Setup
```
Checks all references and connections.

### Force Scene Save
After running any Tools command:
1. File → Save (Ctrl/Cmd + S)
2. Or click Save button in Scene view

---

## 📋 Manual Setup (Last Resort)

If all else fails, set up manually:

1. **Select Start Level Button** in hierarchy

2. **In Inspector, find Button component**

3. **In OnClick() section:**
   - Click **+** to add event
   - Drag **GameManager** from hierarchy into object field
   - In dropdown, select: **LevelManager → BeginLevel()**

4. **Save Scene** (Ctrl/Cmd + S)

---

## 🎯 Expected Console Output

### On Game Start:
```
(nothing - store just opens silently)
```

### On Button Click:
```
BeginLevel() called!
Store closed
Store panel set to inactive
Ammo refilled to 10
Cleared old fortress
Fortress spawned
Level 1 started! levelInProgress = True
```

### If any line is missing, that component has an issue:
- "Store closed" missing → StoreManager problem
- "Ammo refilled" missing → PlayerStats problem
- "Fortress spawned" missing → FortressSpawner problem

---

## ✨ Success Indicators

You'll know it's working when:
- ✅ Console shows "BeginLevel() called!"
- ✅ Store panel disappears
- ✅ Fortress is visible in scene
- ✅ Cannon responds to mouse
- ✅ Firing decreases ammo in HUD
- ✅ No errors in console

---

## 🚀 Next Steps After Button Works

1. Test full level flow: Store → Play → Win → Store
2. Test upgrade purchases
3. Test ammo consumption
4. Test weak shots (when ammo = 0)
5. Test core destruction
6. Verify coins are awarded

---

## 📞 Still Not Working?

If button still doesn't work after all troubleshooting:

1. **Save current scene**
2. **Exit Unity**
3. **Reopen Unity** (forces refresh)
4. **Run Tools → Finalize Game Setup**
5. **Enter Play Mode and test**

Sometimes Unity needs a full refresh to pick up persistent listener changes.
